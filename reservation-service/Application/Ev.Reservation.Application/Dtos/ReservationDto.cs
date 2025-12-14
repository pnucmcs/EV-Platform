using Ev.Reservation.Domain;

namespace Ev.Reservation.Application.Dtos;

public sealed record ReservationDto(Guid Id, Guid UserId, Guid StationId, DateTime StartsAtUtc, DateTime EndsAtUtc, ReservationStatus Status, DateTime CreatedAtUtc);
