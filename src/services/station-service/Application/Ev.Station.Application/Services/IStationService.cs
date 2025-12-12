using Ev.Station.Application.Dtos;
using Ev.Station.Application.Requests;

namespace Ev.Station.Application.Services;

public interface IStationService
{
    Task<IReadOnlyCollection<StationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StationDto> CreateAsync(CreateStationRequest request, CancellationToken cancellationToken = default);
}
