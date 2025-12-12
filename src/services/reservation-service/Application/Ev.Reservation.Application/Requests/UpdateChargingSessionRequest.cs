using Ev.Reservation.Domain;

namespace Ev.Reservation.Application.Requests;

public sealed record UpdateChargingSessionRequest(ChargingSessionStatus Status, DateTime? EndedAtUtc);
