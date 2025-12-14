using Ev.Reservation.Domain;

namespace Ev.Reservation.Application.Dtos;

public sealed record ChargingSessionDto(
    Guid Id,
    Guid ReservationId,
    Guid StationId,
    Guid? ChargerId,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc,
    ChargingSessionStatus Status,
    DateTime CreatedAtUtc);
