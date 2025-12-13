namespace Ev.Station.Domain;

public interface IStationRepository
{
    Task<IReadOnlyCollection<Station>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Station station, IEnumerable<object>? outboxMessages = null, CancellationToken cancellationToken = default);
    Task UpdateAsync(Station station, IEnumerable<object>? outboxMessages = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
