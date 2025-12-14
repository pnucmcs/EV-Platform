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
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid StationId { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Created;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private Reservation()
    {
        // For EF
    }

    private Reservation(Guid id, Guid userId, Guid stationId, DateTime startsAtUtc, DateTime endsAtUtc)
    {
        Id = id;
        UserId = userId;
        StationId = stationId;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
        Validate();
    }

    public static Reservation Create(Guid userId, Guid stationId, DateTime startsAtUtc, DateTime endsAtUtc)
    {
        return new Reservation(Guid.NewGuid(), userId, stationId, startsAtUtc, endsAtUtc);
    }

    public void UpdateStatus(ReservationStatus status)
    {
        Status = status;
    }

    private void Validate()
    {
        if (UserId == Guid.Empty) throw new InvalidOperationException("UserId is required.");
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
        if (StartsAtUtc >= EndsAtUtc) throw new InvalidOperationException("Start must be before end.");
    }
}
