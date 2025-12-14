namespace Ev.Reservation.Application.Requests;

public sealed record CreateChargingSessionRequest(Guid ReservationId, Guid StationId, Guid? ChargerId, DateTime StartedAtUtc);
