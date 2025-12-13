using AutoMapper;
using Ev.AdminConfig.Application.Models;
using Ev.AdminConfig.Application.Requests;
using Ev.AdminConfig.Domain;

namespace Ev.AdminConfig.Application.Services;

public class AdminConfigService : IAdminConfigService
{
    private readonly IAdminConfigRepository _repository;
    private readonly IMapper _mapper;

    public AdminConfigService(IAdminConfigRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<MaintenanceWindowDto> CreateMaintenanceWindowAsync(CreateMaintenanceWindowRequest request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<MaintenanceWindow>(request);
        entity.Validate();
        await _repository.AddMaintenanceWindowAsync(entity, cancellationToken);
        return _mapper.Map<MaintenanceWindowDto>(entity);
    }

    public async Task<bool> DeleteMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetMaintenanceWindowAsync(id, cancellationToken);
        if (existing is null) return false;
        await _repository.DeleteMaintenanceWindowAsync(existing, cancellationToken);
        return true;
    }

    public async Task<MaintenanceWindowDto?> GetMaintenanceWindowAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetMaintenanceWindowAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<MaintenanceWindowDto>(entity);
    }

    public async Task<IReadOnlyList<MaintenanceWindowDto>> GetMaintenanceWindowsAsync(Guid stationId, CancellationToken cancellationToken)
    {
        var windows = await _repository.GetMaintenanceWindowsAsync(stationId, cancellationToken);
        return _mapper.Map<IReadOnlyList<MaintenanceWindowDto>>(windows);
    }

    public async Task<MaintenanceWindowDto?> UpdateMaintenanceWindowAsync(Guid id, UpdateMaintenanceWindowRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetMaintenanceWindowAsync(id, cancellationToken);
        if (existing is null) return null;
        _mapper.Map(request, existing);
        existing.Validate();
        await _repository.UpdateMaintenanceWindowAsync(existing, cancellationToken);
        return _mapper.Map<MaintenanceWindowDto>(existing);
    }

    public async Task<StationOpsConfigDto> UpsertOpsConfigAsync(StationOpsConfigRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetOpsConfigAsync(request.StationId, cancellationToken);
        if (existing is null)
        {
            existing = _mapper.Map<StationOpsConfig>(request);
        }
        else
        {
            existing.AllowCharging = request.AllowCharging;
            existing.AllowReservations = request.AllowReservations;
        }
        existing.UpdatedAtUtc = DateTime.UtcNow;
        existing.Validate();
        await _repository.UpsertOpsConfigAsync(existing, cancellationToken);
        return _mapper.Map<StationOpsConfigDto>(existing);
    }

    public async Task<StationOpsConfigDto?> GetOpsConfigAsync(Guid stationId, CancellationToken cancellationToken)
    {
        var config = await _repository.GetOpsConfigAsync(stationId, cancellationToken);
        return config is null ? null : _mapper.Map<StationOpsConfigDto>(config);
    }
}
