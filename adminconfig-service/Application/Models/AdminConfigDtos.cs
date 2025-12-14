namespace Ev.AdminConfig.Application.Models;

public record MaintenanceWindowDto(Guid Id, Guid StationId, DateTime StartUtc, DateTime EndUtc, string Reason);

public record StationOpsConfigDto(Guid StationId, bool AllowReservations, bool AllowCharging, DateTime UpdatedAtUtc);
