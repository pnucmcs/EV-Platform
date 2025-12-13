using System.ComponentModel.DataAnnotations;

namespace Ev.AdminConfig.Application.Requests;

public class CreateMaintenanceWindowRequest
{
    [Required] public Guid StationId { get; set; }
    [Required] public DateTime StartUtc { get; set; }
    [Required] public DateTime EndUtc { get; set; }
    [Required] public string Reason { get; set; } = string.Empty;
}

public class UpdateMaintenanceWindowRequest : CreateMaintenanceWindowRequest
{
}

public class StationOpsConfigRequest
{
    [Required] public Guid StationId { get; set; }
    public bool AllowReservations { get; set; } = true;
    public bool AllowCharging { get; set; } = true;
}
