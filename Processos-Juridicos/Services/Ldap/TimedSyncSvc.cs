using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.Services.Interfaces.Ldap;

namespace Processos_Juridicos.Services.Ldap;

public class TimedSyncSvc(IServiceProvider services, ILogger<TimedSyncSvc> logger) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // adjust as needed
    private readonly ILogger<TimedSyncSvc> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunRoleSyncTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Timed role sync run failed");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunRoleSyncTickAsync(CancellationToken ct)
    {
        using IServiceScope scope = _services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        RoleSyncSvc roleSync = scope.ServiceProvider.GetRequiredService<RoleSyncSvc>();
        ILdapUserSvc ldap = scope.ServiceProvider.GetRequiredService<ILdapUserSvc>();

        // Get NIIs from DB
        List<string> dbNiis = await db.Users
            .Where(u => !string.IsNullOrEmpty(u.UserNii))
            .Select(u => u.UserNii!)
            .Distinct()
            .ToListAsync(ct);

        // Get NIIs from LDAP
        var ldapNiis = roleSync.ManagedGroupNames
            .SelectMany(ldap.GetEmployeeIdsInGroup)
            .Where(nii => !string.IsNullOrWhiteSpace(nii))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Merge and log
        var allNiis = dbNiis.Union(ldapNiis, StringComparer.OrdinalIgnoreCase).ToList();
        _logger.LogInformation("Role sync tick: DB={DbCount}, LDAP={LdapCount}, Total={Total}",
            dbNiis.Count, ldapNiis.Count, allNiis.Count);

        // Process NIIs
        foreach (var nii in allNiis)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            await roleSync.SyncUserRolesAsync(nii, ct);
        }

        _logger.LogInformation("Role sync tick completed at {Time}", DateTimeOffset.Now);
    }

}
