namespace Ev.Reservation.Domain;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid stationId, DateTime startsAtUtc, DateTime endsAtUtc, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChargingSession>> GetSessionsAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task<ChargingSession?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task AddSessionAsync(ChargingSession session, CancellationToken cancellationToken = default);
    Task UpdateSessionAsync(ChargingSession session, CancellationToken cancellationToken = default);
    Task<bool> HasActiveSessionAsync(Guid reservationId, CancellationToken cancellationToken = default);
}
