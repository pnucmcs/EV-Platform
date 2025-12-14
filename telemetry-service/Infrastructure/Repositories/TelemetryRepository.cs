using Ev.Telemetry.Domain;
using Ev.Telemetry.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Telemetry.Infrastructure.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly TelemetryDbContext _db;

    public TelemetryRepository(TelemetryDbContext db)
    {
        _db = db;
    }

    public async Task AddReadingsAsync(IEnumerable<TelemetryReading> readings, CancellationToken cancellationToken)
    {
        await _db.TelemetryReadings.AddRangeAsync(readings, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<TelemetryReading?> GetLatestForStationAsync(Guid stationId, CancellationToken cancellationToken)
    {
        return _db.TelemetryReadings
            .Where(r => r.StationId == stationId)
            .OrderByDescending(r => r.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
