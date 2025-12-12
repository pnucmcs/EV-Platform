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
}
