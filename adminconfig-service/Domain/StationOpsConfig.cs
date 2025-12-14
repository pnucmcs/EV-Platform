namespace Ev.AdminConfig.Domain;

public class StationOpsConfig
{
    public Guid StationId { get; set; }
    public bool AllowReservations { get; set; } = true;
    public bool AllowCharging { get; set; } = true;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public void Validate()
    {
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
        if (UpdatedAtUtc.Kind != DateTimeKind.Utc) throw new InvalidOperationException("UpdatedAtUtc must be UTC.");
    }
}
