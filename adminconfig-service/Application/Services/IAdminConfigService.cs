using Ev.AdminConfig.Application.Models;
using Ev.AdminConfig.Application.Requests;

namespace Ev.AdminConfig.Application.Services;

public interface IAdminConfigService
{
    Task<IReadOnlyList<MaintenanceWindowDto>> GetMaintenanceWindowsAsync(Guid stationId, CancellationToken cancellationToken);
    Task<MaintenanceWindowDto?> GetMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken);
    Task<MaintenanceWindowDto> CreateMaintenanceWindowAsync(CreateMaintenanceWindowRequest request, CancellationToken cancellationToken);
    Task<MaintenanceWindowDto?> UpdateMaintenanceWindowAsync(Guid id, UpdateMaintenanceWindowRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken);

    Task<StationOpsConfigDto> UpsertOpsConfigAsync(StationOpsConfigRequest request, CancellationToken cancellationToken);
    Task<StationOpsConfigDto?> GetOpsConfigAsync(Guid stationId, CancellationToken cancellationToken);
}
