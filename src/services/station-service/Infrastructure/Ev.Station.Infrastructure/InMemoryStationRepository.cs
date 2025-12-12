using Ev.Station.Domain;

namespace Ev.Station.Infrastructure;

public sealed class InMemoryStationRepository : IStationRepository
{
    private readonly List<Ev.Station.Domain.Station> _stations = new();
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task AddAsync(Ev.Station.Domain.Station station, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            _stations.Add(station);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyCollection<Ev.Station.Domain.Station>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return _stations.ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Ev.Station.Domain.Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return _stations.FirstOrDefault(s => s.Id == id);
        }
        finally
        {
            _gate.Release();
        }
    }
}
