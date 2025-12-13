using System.ComponentModel.DataAnnotations;

namespace Ev.Telemetry.Application.Requests;

public class TelemetryReadingRequest
{
    [Required] public string DeviceId { get; set; } = string.Empty;
    [Required] public Guid StationId { get; set; }
    [Required] public DateTime TimestampUtc { get; set; }
    public decimal Voltage { get; set; }
    public decimal PowerKw { get; set; }
    public decimal EnergyKwhDelta { get; set; }
    public decimal TemperatureC { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BulkTelemetryRequest
{
    [Required]
    public List<TelemetryReadingRequest> Readings { get; set; } = new();
}
