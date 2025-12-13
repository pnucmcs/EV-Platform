using Ev.Telemetry.Application.Models;
using Ev.Telemetry.Application.Requests;

namespace Ev.Telemetry.Application.Services;

public interface ITelemetryService
{
    Task IngestAsync(IEnumerable<TelemetryReadingRequest> readings, CancellationToken cancellationToken);
    Task<TelemetryReadingDto?> GetLatestForStationAsync(Guid stationId, CancellationToken cancellationToken);
}
