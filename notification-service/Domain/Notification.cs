namespace Ev.Notification.Domain;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Type)) throw new InvalidOperationException("Type is required.");
        if (string.IsNullOrWhiteSpace(Title)) throw new InvalidOperationException("Title is required.");
        if (EventId == Guid.Empty) throw new InvalidOperationException("EventId is required for idempotency.");
    }
}
