namespace Ev.Reservation.Application.Requests;

public sealed record CreateReservationRequest(Guid UserId, Guid StationId, DateTime StartsAtUtc, DateTime EndsAtUtc);
