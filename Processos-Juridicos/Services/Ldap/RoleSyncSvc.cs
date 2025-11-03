using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Services.Ldap;

public class RoleSyncSvc
{
    private const string oficialInstrutorRoleName = "OFICIAIS-INSTRUTORES";
    private const string comandoUnidadeRoleName = "COMANDO-UNIDADE";
    private const string djUnallowedRoleName = "DJ-UNAUTHORIZED";
    private const string djAllowedRoleName = "DJ-AUTHORIZED";
    private const string superRoleName = "SUPERADMIN";

    private readonly AppDbContext _db;
    private readonly ILogger<RoleSyncSvc> _logger;

    private readonly IUserDataSvc _usvc;



    private readonly HashSet<int> _nonManagedRoles;

    private readonly HashSet<string> _wantedGroups;

    private readonly Dictionary<string, int> _groupRoleMapping;

    private readonly Dictionary<int, int> _rolePriority;

    public RoleSyncSvc(AppDbContext db, IUserDataSvc usvc, ILogger<RoleSyncSvc> logger)
    {
        _db = db;
        _logger = logger;
        _usvc = usvc;

        var _oficialInstrutorRoleId = (int)db.Roles.Single(r => r.RoleName == oficialInstrutorRoleName).RoleId!;
        var _comandoUnidadeRoleId = (int)db.Roles.Single(r => r.RoleName == comandoUnidadeRoleName).RoleId!;
        var _djUnallowedRoleId = (int)db.Roles.Single(r => r.RoleName == djUnallowedRoleName).RoleId!;
        var _djAllowedRoleId = (int)db.Roles.Single(r => r.RoleName == djAllowedRoleName).RoleId!;
        var _superRoleId = (int)db.Roles.Single(r => r.RoleName == superRoleName).RoleId!;

        _nonManagedRoles = [_djAllowedRoleId, _superRoleId];

        _wantedGroups = new(StringComparer.OrdinalIgnoreCase)
        {
            "CN=MARINHA-OFICIAIS-ATIVO,CN=Users,DC=marinha,DC=pt"
        };

        _rolePriority = new()
        {
            { _djUnallowedRoleId, 1 },
            { _comandoUnidadeRoleId, 2 },
            { _oficialInstrutorRoleId, 3 }
        };

        _groupRoleMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MARINHA-OFICIAIS-ATIVO"] = _oficialInstrutorRoleId,
            ["COMANDO-UNIDADE"] = _comandoUnidadeRoleId,
            ["DJ-GERAL"] = _djUnallowedRoleId,
        };
    }

    public IReadOnlyCollection<string> ManagedGroupNames => _wantedGroups;

    public async Task SyncUserRolesAsync(string userNii, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userNii))
        {
            return;
        }

        UserDataModel? searchedUser = await _usvc.GetUserByNiiAsync(userNii);

        if (searchedUser == null)
        {
            UserDataModel? ldapUser = await _usvc.GetUserByNiiAsync(userNii);
            if (ldapUser != null)
            {
                searchedUser = new UserDataModel
                {
                    Nii = userNii,
                    UserName = ldapUser.DisplayName,
                    Groups = ldapUser.Groups,
                    Unit = ldapUser.Unit
                };
            }
            else
            {
                return;
            }
        }

        var groupSet = new HashSet<string>(
            (searchedUser.Groups ?? Enumerable.Empty<string>()).Select(ExtractCn),
            StringComparer.OrdinalIgnoreCase
        );

        List<int> currentRoles = await _db.Users
            .Where(u => u.UserNii == userNii && u.RoleId != null)
            .Select(u => u.RoleId!.Value)
            .ToListAsync(ct);

        var isOverride = await _db.Users
            .Where(u => u.UserNii == userNii)
            .Select(u => u.IsUserManuallySet)
            .FirstOrDefaultAsync(ct);

        if (currentRoles.Any(_nonManagedRoles.Contains) || isOverride)
        {
            _logger.LogInformation("User {UserNii} has Super/Authorized or is a manual override; skipping sync.", userNii);
            return;
        }

        List<int> desiredRoles = CalculateSyncDesiredRoles(groupSet);

        _logger.LogInformation("User {UserNii} desired roles: {DesiredRoles}",
            userNii, string.Join(",", desiredRoles));

        (List<int> toAdd, List<int> toRemove) = DiffSyncRoles(currentRoles, desiredRoles);

        await ApplySyncRoleChangesAsync(userNii, searchedUser.DisplayName!, toAdd, toRemove, ct);
    }

    private List<int> CalculateSyncDesiredRoles(HashSet<string> groupSet)
    {
        var desiredRoles = _groupRoleMapping
            .Where(kvp => groupSet.Contains(kvp.Key))
            .Select(kvp => kvp.Value)
            .Where(roleId => !_nonManagedRoles.Contains(roleId))
            .Distinct()
            .ToList();

        _logger.LogInformation("User has groups: {Groups}", string.Join(", ", groupSet));

        // Apply priority: keep only the top‑priority role(s)
        if (desiredRoles.Count > 1)
        {
            desiredRoles = [.. desiredRoles
                .OrderBy(r => _rolePriority.TryGetValue(r, out var p) ? p : int.MaxValue)
                .Take(1)];
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



    private async Task ApplySyncRoleChangesAsync(
        string userNii,
        string userName,
        List<int> toAdd,
        List<int> toRemove,
        CancellationToken ct)
    {
        User? user = await _db.Users
            .FirstOrDefaultAsync(u => u.UserNii == userNii, ct);


        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            return;
        }

        if (toAdd.Count == 1 && user != null)
        {
            var newRoleId = toAdd[0];

            var oldRoleId = user.RoleId;

            if (oldRoleId != newRoleId)
            {
                user.RoleId = newRoleId;
                user.UserName = userName;
                _db.Users.Update(user);

                _logger.LogInformation("Updated role for {UserNii}: {OldRole} -> {NewRole}",
                    userNii, oldRoleId?.ToString() ?? "none", newRoleId);
            }
            else
            {
                _logger.LogInformation("User {UserNii} already has role {RoleId}; no update", userNii, newRoleId);
            }

            await _db.SaveChangesAsync(ct);
            return;
        }

        if (toAdd.Count == 1 && user == null)
        {
            var newRoleId = toAdd[0];

            var newUser = new User
            {
                UserNii = userNii,
                UserName = userName,
                RoleId = newRoleId
            };

            _db.Users.Add(newUser);
            _logger.LogInformation("Added new user {UserNii} with role {RoleId}", userNii, newRoleId);

            await _db.SaveChangesAsync(ct);
            return;
        }

        if (toAdd.Count == 0 && toRemove.Count == 1 && user != null)
        {
            _db.Users.Remove(user);
            _logger.LogInformation("Removed user {UserNii} because it has no roles", userNii);

            await _db.SaveChangesAsync(ct);
            return;
        }

        _logger.LogWarning(
            "Unexpected diff for {UserNii}: toAdd=[{Add}] toRemove=[{Remove}]; no changes applied",
            userNii, string.Join(",", toAdd), string.Join(",", toRemove));
    }



    private static string ExtractCn(string distinguishedName)
    {
        if (string.IsNullOrWhiteSpace(distinguishedName))
        {
            return distinguishedName;
        }

        if (distinguishedName.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = distinguishedName.IndexOf(',');
            return commaIndex > 3
                ? distinguishedName[3..commaIndex]
                : distinguishedName[3..];
        }

        return distinguishedName;
    }


}
