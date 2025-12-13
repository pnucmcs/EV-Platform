namespace Ev.Reporting.Domain.Repositories;

public interface IReportingRepository
{
    Task<IReadOnlyList<StationUtilizationDaily>> GetDailyRangeAsync(Guid stationId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
    Task<StationUtilizationDaily> GetOrCreateDailyAsync(Guid stationId, DateOnly date, CancellationToken cancellationToken);
    Task<bool> HasProcessedEventAsync(Guid eventId, CancellationToken cancellationToken);
    Task MarkEventProcessedAsync(Guid eventId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
