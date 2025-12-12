namespace Ev.Reservation.Domain;

public enum ChargingSessionStatus
{
    Created = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public sealed class ChargingSession
{
    public Guid Id { get; private set; }
    public Guid ReservationId { get; private set; }
    public Guid StationId { get; private set; }
    public Guid? ChargerId { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }
    public ChargingSessionStatus Status { get; private set; } = ChargingSessionStatus.Created;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private ChargingSession()
    {
    }

    private ChargingSession(Guid id, Guid reservationId, Guid stationId, Guid? chargerId, DateTime startedAtUtc)
    {
        Id = id;
        ReservationId = reservationId;
        StationId = stationId;
        ChargerId = chargerId;
        StartedAtUtc = startedAtUtc;
        Validate();
    }

    public static ChargingSession Start(Guid reservationId, Guid stationId, Guid? chargerId, DateTime startedAtUtc)
    {
        return new ChargingSession(Guid.NewGuid(), reservationId, stationId, chargerId, startedAtUtc);
    }

    public void Complete(DateTime endedAtUtc)
    {
        if (endedAtUtc < StartedAtUtc) throw new InvalidOperationException("End time cannot be before start time.");
        EndedAtUtc = endedAtUtc;
        Status = ChargingSessionStatus.Completed;
    }

    public void Fail(string? reason = null)
    {
        Status = ChargingSessionStatus.Failed;
    }

    public void Cancel()
    {
        Status = ChargingSessionStatus.Cancelled;
    }

    private void Validate()
    {
        if (ReservationId == Guid.Empty) throw new InvalidOperationException("ReservationId is required.");
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
    }
}
