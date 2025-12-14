namespace Ev.Shared.Messaging.Events;

public sealed record ReservationCreated(Guid ReservationId, Guid UserId, Guid StationId, DateTime StartsAtUtc, DateTime EndsAtUtc);
