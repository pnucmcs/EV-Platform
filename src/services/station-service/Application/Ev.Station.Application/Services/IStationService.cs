using Ev.Station.Application.Dtos;
using Ev.Station.Application.Requests;

namespace Ev.Station.Application.Services;

public interface IStationService
{
    Task<IReadOnlyCollection<StationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StationDto> CreateAsync(CreateStationRequest request, CancellationToken cancellationToken = default);
    Task<StationDto?> UpdateAsync(Guid id, UpdateStationRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChargerDto>?> GetChargersAsync(Guid stationId, CancellationToken cancellationToken = default);
    Task<ChargerDto?> GetChargerByIdAsync(Guid stationId, Guid chargerId, CancellationToken cancellationToken = default);
    Task<ChargerDto?> CreateChargerAsync(Guid stationId, CreateChargerRequest request, CancellationToken cancellationToken = default);
    Task<ChargerDto?> UpdateChargerAsync(Guid stationId, Guid chargerId, UpdateChargerRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteChargerAsync(Guid stationId, Guid chargerId, CancellationToken cancellationToken = default);
}
