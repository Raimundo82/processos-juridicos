using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Services.Interfaces.Auth;

namespace Processos_Juridicos.Services.Auth;

public class RoleSyncService
{
    private const string oficialInstrutorRoleName = "OFICIAIS-INSTRUTORES";
    private const string comandoUnidadeRoleName = "COMANDO-UNIDADE";
    private const string djUnallowedRoleName = "DJ-UNAUTHORIZED";
    private const string djAllowedRoleName = "DJ-AUTHORIZED";
    private const string superRoleName = "SUPERADMIN";

    private readonly AppDbContext _db;
    private readonly ILdapUserSvc _ldap;
    private readonly ILogger<RoleSyncService> _logger;

    private readonly int _djUnallowedRoleId;

    private readonly HashSet<int> _nonManagedRoles;

    private readonly Dictionary<string, int> _groupRoleMap;

    public RoleSyncService(AppDbContext db, ILdapUserSvc ldap, ILogger<RoleSyncService> logger)
    {
        _db = db;
        _ldap = ldap;
        _logger = logger;

        var _oficialInstrutorRoleId = (int)db.Roles.Single(r => r.RoleName == oficialInstrutorRoleName).RoleId!;
        var _comandoUnidadeRoleId = (int)db.Roles.Single(r => r.RoleName == comandoUnidadeRoleName).RoleId!;
        _djUnallowedRoleId = (int)db.Roles.Single(r => r.RoleName == djUnallowedRoleName).RoleId!;
        var _djAllowedRoleId = (int)db.Roles.Single(r => r.RoleName == djAllowedRoleName).RoleId!;
        var _superRoleId = (int)db.Roles.Single(r => r.RoleName == superRoleName).RoleId!;

        _nonManagedRoles = [_djAllowedRoleId, _superRoleId];

        _groupRoleMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MARINHA-OFICIAIS-ATIVO"] = _oficialInstrutorRoleId, // Oficial Instrutor
            ["COMANDO-UNIDADE"] = _comandoUnidadeRoleId,   // Comando Unidade
        };
    }

    public IReadOnlyCollection<string> ManagedGroupNames => [.. _groupRoleMap.Keys];

    public async Task SyncUserRolesAsync(string userNii, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userNii))
        {
            return;
        }

        // Resolve username
        IReadOnlyList<Models.UserDataModel> candidates = _ldap.SearchUsers(userNii, take: 5);
        Models.UserDataModel? byEmployeeId = candidates.FirstOrDefault(u =>
            !string.IsNullOrEmpty(u.Nii) &&
            string.Equals(u.Nii, userNii, StringComparison.OrdinalIgnoreCase));
        var username = byEmployeeId?.UserName ?? userNii;

        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        // Fetch groups
        var groupSet = new HashSet<string>(_ldap.GetUserGroups(username) ?? [], StringComparer.OrdinalIgnoreCase);

        // Resolve department
        var department = !string.IsNullOrWhiteSpace(byEmployeeId?.Unit)
            ? byEmployeeId.Unit
            : _ldap.SearchUsers(username, take: 1)
                .FirstOrDefault(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase))
                ?.Unit;

        // Current roles
        List<int> currentRoles = await _db.Users
            .Where(u => u.UserNii == userNii && u.RoleId != null)
            .Select(u => u.RoleId!.Value)
            .ToListAsync(ct);

        var isOverride = await _db.Users
            .Where(u => u.UserNii == userNii)
            .Select(u => u.IsUserManuallySet)
            .FirstOrDefaultAsync(ct);

        // Skip if has non-managed roles
        if (currentRoles.Any(_nonManagedRoles.Contains) || isOverride)
        {
            _logger.LogInformation("User {UserNii} has Super/Authorized or is a manual override; skipping sync.", userNii);
            return;
        }

        // Determine desired roles
        List<int> desiredRoles = CalculateSyncDesiredRoles(groupSet, department);

        // Diff
        (List<int> toAdd, List<int> toRemove) = DiffSyncRoles(currentRoles, desiredRoles);

        // Apply changes
        await ApplySyncRoleChangesAsync(userNii, username, toAdd, toRemove, ct);
    }

    private List<int> CalculateSyncDesiredRoles(HashSet<string> groupSet, string? department)
    {
        var desiredRoles = _groupRoleMap
            .Where(kvp => groupSet.Contains(kvp.Key))
            .Select(kvp => kvp.Value)
            .Where(roleId => !_nonManagedRoles.Contains(roleId))
            .Distinct()
            .ToList();

        if (string.Equals(department, "DJ", StringComparison.OrdinalIgnoreCase) && desiredRoles.Count == 0)
        {
            desiredRoles.Add(_djUnallowedRoleId);
        }

        return desiredRoles;
    }

    private (List<int> ToAdd, List<int> ToRemove) DiffSyncRoles(List<int> currentRoles, List<int> desiredRoles)
    {
        var managedCurrent = currentRoles.Where(r => !_nonManagedRoles.Contains(r)).ToList();
        var toAdd = desiredRoles.Except(managedCurrent).ToList();
        var toRemove = managedCurrent.Except(desiredRoles).ToList();
        return (toAdd, toRemove);
    }

    private async Task ApplySyncRoleChangesAsync(string userNii, string userName, List<int> toAdd, List<int> toRemove, CancellationToken ct)
    {
        if (toAdd.Count > 0)
        {
            foreach (var roleId in toAdd)
            {
                _db.Users.Add(new User { UserNii = userNii, RoleId = roleId, UserName = userName });
            }
        }

        if (toRemove.Count > 0)
        {
            List<User> removeEntities = await _db.Users
                .Where(u => u.UserNii == userNii && u.RoleId != null && toRemove.Contains(u.RoleId.Value))
                .ToListAsync(ct);

            _db.Users.RemoveRange(removeEntities);
        }

        if (toAdd.Count > 0 || toRemove.Count > 0)
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Updated roles for {UserNii}: +{Added} -{Removed}",
                userNii, string.Join(",", toAdd), string.Join(",", toRemove));
        }
        else
        {
            _logger.LogDebug("No role changes for {UserNii}", userNii);
        }
    }

}
