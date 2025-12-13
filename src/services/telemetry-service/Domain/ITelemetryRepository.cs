namespace Ev.Telemetry.Domain;

public interface ITelemetryRepository
{
    Task AddReadingsAsync(IEnumerable<TelemetryReading> readings, CancellationToken cancellationToken);
    Task<TelemetryReading?> GetLatestForStationAsync(Guid stationId, CancellationToken cancellationToken);
}
