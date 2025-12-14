using Ev.AdminConfig.Domain;
using Ev.AdminConfig.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.AdminConfig.Infrastructure.Repositories;

public class AdminConfigRepository : IAdminConfigRepository
{
    private readonly AdminConfigDbContext _db;

    public AdminConfigRepository(AdminConfigDbContext db)
    {
        _db = db;
    }

    public async Task AddMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken)
    {
        await _db.MaintenanceWindows.AddAsync(window, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken)
    {
        _db.MaintenanceWindows.Remove(window);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<MaintenanceWindow?> GetMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.MaintenanceWindows.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MaintenanceWindow>> GetMaintenanceWindowsAsync(Guid stationId, CancellationToken cancellationToken)
    {
        return await _db.MaintenanceWindows.Where(x => x.StationId == stationId)
            .OrderBy(x => x.StartUtc).ToListAsync(cancellationToken);
    }

    public Task<StationOpsConfig?> GetOpsConfigAsync(Guid stationId, CancellationToken cancellationToken)
    {
        return _db.StationOpsConfigs.FirstOrDefaultAsync(x => x.StationId == stationId, cancellationToken);
    }

    public async Task UpdateMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken)
    {
        _db.MaintenanceWindows.Update(window);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertOpsConfigAsync(StationOpsConfig config, CancellationToken cancellationToken)
    {
        var existing = await _db.StationOpsConfigs.FirstOrDefaultAsync(x => x.StationId == config.StationId, cancellationToken);
        if (existing is null)
        {
            await _db.StationOpsConfigs.AddAsync(config, cancellationToken);
        }
        else
        {
            existing.AllowCharging = config.AllowCharging;
            existing.AllowReservations = config.AllowReservations;
            existing.UpdatedAtUtc = config.UpdatedAtUtc;
            _db.StationOpsConfigs.Update(existing);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
