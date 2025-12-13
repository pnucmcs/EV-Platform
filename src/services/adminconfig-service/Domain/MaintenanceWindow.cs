namespace Ev.AdminConfig.Domain;

public class MaintenanceWindow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StationId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Reason { get; set; } = string.Empty;

    public void Validate()
    {
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
        if (StartUtc.Kind != DateTimeKind.Utc || EndUtc.Kind != DateTimeKind.Utc)
            throw new InvalidOperationException("Times must be UTC.");
        if (EndUtc <= StartUtc) throw new InvalidOperationException("EndUtc must be after StartUtc.");
    }
}
