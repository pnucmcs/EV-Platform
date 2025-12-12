namespace Ev.Station.Domain;

public interface IStationRepository
{
    Task<IReadOnlyCollection<Station>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Station station, CancellationToken cancellationToken = default);
}
