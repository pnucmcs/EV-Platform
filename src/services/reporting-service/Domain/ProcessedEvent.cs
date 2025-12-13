namespace Ev.Reporting.Domain;

public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}
