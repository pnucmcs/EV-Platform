using Ev.Station.Domain;
using Ev.Station.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Station.Infrastructure;

public sealed class StationRepository : IStationRepository
{
    private readonly StationDbContext _db;

    public StationRepository(StationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Domain.Station station, CancellationToken cancellationToken = default)
    {
        _db.Stations.Add(station);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Domain.Station>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Stations
            .Include(x => x.Chargers)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Stations
            .Include(x => x.Chargers)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Domain.Station station, CancellationToken cancellationToken = default)
    {
        _db.Stations.Update(station);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var station = await _db.Stations
            .Include(x => x.Chargers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (station is null)
        {
            return;
        }

        _db.Stations.Remove(station);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
