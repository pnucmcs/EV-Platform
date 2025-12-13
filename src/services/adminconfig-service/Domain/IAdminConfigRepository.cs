namespace Ev.AdminConfig.Domain;

public interface IAdminConfigRepository
{
    Task<IReadOnlyList<MaintenanceWindow>> GetMaintenanceWindowsAsync(Guid stationId, CancellationToken cancellationToken);
    Task<MaintenanceWindow?> GetMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken);
    Task AddMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken);
    Task UpdateMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken);
    Task DeleteMaintenanceWindowAsync(MaintenanceWindow window, CancellationToken cancellationToken);

    Task<StationOpsConfig?> GetOpsConfigAsync(Guid stationId, CancellationToken cancellationToken);
    Task UpsertOpsConfigAsync(StationOpsConfig config, CancellationToken cancellationToken);
}
