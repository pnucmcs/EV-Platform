using Ev.Reservation.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Reservation.Infrastructure;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly ReservationDbContext _db;

    public ReservationRepository(ReservationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Domain.Reservation reservation, CancellationToken cancellationToken = default)
    {
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Domain.Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Reservations.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<Domain.Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Reservations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Domain.Reservation reservation, CancellationToken cancellationToken = default)
    {
        _db.Reservations.Update(reservation);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(Guid stationId, DateTime startsAtUtc, DateTime endsAtUtc, CancellationToken cancellationToken = default)
    {
        return await _db.Reservations
            .AsNoTracking()
            .AnyAsync(r =>
                r.StationId == stationId &&
                r.EndsAtUtc > startsAtUtc &&
                r.StartsAtUtc < endsAtUtc,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChargingSession>> GetSessionsAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var list = await _db.ChargingSessions.AsNoTracking().Where(x => x.ReservationId == reservationId).ToListAsync(cancellationToken);
        return list;
    }

    public async Task<ChargingSession?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _db.ChargingSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public async Task AddSessionAsync(ChargingSession session, CancellationToken cancellationToken = default)
    {
        _db.ChargingSessions.Add(session);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSessionAsync(ChargingSession session, CancellationToken cancellationToken = default)
    {
        _db.ChargingSessions.Update(session);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasActiveSessionAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await _db.ChargingSessions
            .AsNoTracking()
            .AnyAsync(s =>
                s.ReservationId == reservationId &&
                (s.Status == ChargingSessionStatus.Created || s.Status == ChargingSessionStatus.InProgress),
                cancellationToken);
    }
}
