namespace Ev.Reservation.Domain;

public enum ReservationStatus
{
    Created = 0,
    Started = 1,
    Completed = 2,
    Cancelled = 3
}

public sealed class Reservation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StationId { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Created;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
