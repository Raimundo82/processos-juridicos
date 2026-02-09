using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Services.Ldap;

public class TimedSyncSvc(IServiceProvider services, ILogger<TimedSyncSvc> logger) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<TimedSyncSvc> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily sync service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime nextRun = DateTime.Today.AddHours(3);
            if (DateTime.Now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            TimeSpan delay = nextRun - DateTime.Now;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Next sync at {NextRun}", nextRun);
            }

            await Task.Delay(delay, stoppingToken);
            await ExecuteSyncSafely(stoppingToken);
        }
    }

    private async Task ExecuteSyncSafely(CancellationToken ct)
    {
        try
        {
            await RunRoleSyncTickAsync(ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Graceful shutdown, no logging needed
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Weekly sync failed at {Time}", DateTimeOffset.Now);
        }
    }

    private async Task RunRoleSyncTickAsync(CancellationToken ct)
    {
        using IServiceScope scope = _services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        RoleSyncSvc roleSync = scope.ServiceProvider.GetRequiredService<RoleSyncSvc>();
        IUserDataSvc userDataSvc = scope.ServiceProvider.GetRequiredService<IUserDataSvc>();

        List<string> dbNiis = await db.Users
            .Where(u => !string.IsNullOrEmpty(u.UserNii))
            .Select(u => u.UserNii!)
            .Distinct()
            .ToListAsync(ct);

        var ldapNiis = roleSync.ManagedGroupNames
     .SelectMany(userDataSvc.GetUsersInGroup)
     .Where(nii => !string.IsNullOrWhiteSpace(nii))
     .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allNiis = dbNiis.Union(ldapNiis, StringComparer.OrdinalIgnoreCase).ToList();
        _logger.LogInformation("Role sync tick: DB={DbCount}, LDAP={LdapCount}, Total={Total}",
            dbNiis.Count, ldapNiis.Count, allNiis.Count);

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
