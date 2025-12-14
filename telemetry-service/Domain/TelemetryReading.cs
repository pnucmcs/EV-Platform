namespace Ev.Telemetry.Domain;

public class TelemetryReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DeviceId { get; set; } = string.Empty;
    public Guid StationId { get; set; }
    public DateTime TimestampUtc { get; set; }
    public decimal Voltage { get; set; }
    public decimal PowerKw { get; set; }
    public decimal EnergyKwhDelta { get; set; }
    public decimal TemperatureC { get; set; }
    public string Status { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DeviceId)) throw new InvalidOperationException("DeviceId is required.");
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
        if (TimestampUtc.Kind != DateTimeKind.Utc) throw new InvalidOperationException("Timestamp must be UTC.");
    }
}
