using Ev.Reservation.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Reservation.Infrastructure;

public sealed class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation.Domain.Reservation> Reservations => Set<Reservation.Domain.Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation.Domain.Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.StationId).IsRequired();
            entity.Property(x => x.StartsAtUtc).IsRequired();
            entity.Property(x => x.EndsAtUtc).IsRequired();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });
    }
}
