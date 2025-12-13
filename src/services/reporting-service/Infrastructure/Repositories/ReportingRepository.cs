using Ev.Reporting.Domain;
using Ev.Reporting.Domain.Repositories;
using Ev.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Reporting.Infrastructure.Repositories;

public class ReportingRepository : IReportingRepository
{
    private readonly ReportingDbContext _dbContext;

    public ReportingRepository(ReportingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StationUtilizationDaily>> GetDailyRangeAsync(Guid stationId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var query = _dbContext.StationUtilizations.AsQueryable()
            .Where(x => x.StationId == stationId);

        if (from.HasValue)
        {
            query = query.Where(x => x.Date >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.Date <= to.Value);
        }

        return await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);
    }

    public async Task<StationUtilizationDaily> GetOrCreateDailyAsync(Guid stationId, DateOnly date, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.StationUtilizations.FirstOrDefaultAsync(x => x.StationId == stationId && x.Date == date, cancellationToken);
        if (existing is not null) return existing;

        var entity = new StationUtilizationDaily
        {
            StationId = stationId,
            Date = date,
            SessionsCount = 0,
            TotalKwh = 0,
            TotalRevenue = 0,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _dbContext.StationUtilizations.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<bool> HasProcessedEventAsync(Guid eventId, CancellationToken cancellationToken)
    {
        return _dbContext.ProcessedEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);
    }

    public async Task MarkEventProcessedAsync(Guid eventId, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessedEvents.AddAsync(new ProcessedEvent { EventId = eventId, ProcessedAtUtc = DateTime.UtcNow }, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
