using Ev.Reporting.Application.Models;

namespace Ev.Reporting.Application.Services;

public interface IReportingService
{
    Task<IReadOnlyList<StationDailyReportDto>> GetStationDailyAsync(Guid stationId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
}
