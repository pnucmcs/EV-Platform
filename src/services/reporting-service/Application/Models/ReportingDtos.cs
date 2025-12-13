namespace Ev.Reporting.Application.Models;

public record StationDailyReportDto(Guid StationId, DateOnly Date, int SessionsCount, double TotalKwh, decimal TotalRevenue);
