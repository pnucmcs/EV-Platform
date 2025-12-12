using Ev.Reservation.Application.Dtos;
using Ev.Reservation.Application.Requests;

namespace Ev.Reservation.Application.Services;

public interface IReservationService
{
    Task<IReadOnlyCollection<ReservationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ReservationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReservationDto> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
}
