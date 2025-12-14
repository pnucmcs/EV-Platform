using Ev.Reservation.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Reservation.Infrastructure;

public sealed class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation.Domain.Reservation> Reservations => Set<Reservation.Domain.Reservation>();
    public DbSet<ChargingSession> ChargingSessions => Set<ChargingSession>();
    public DbSet<Reservation.Domain.OutboxMessage> OutboxMessages => Set<Reservation.Domain.OutboxMessage>();

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

        modelBuilder.Entity<ChargingSession>(entity =>
        {
            entity.ToTable("charging_sessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ReservationId).IsRequired();
            entity.Property(x => x.StationId).IsRequired();
            entity.Property(x => x.ChargerId);
            entity.Property(x => x.StartedAtUtc).IsRequired();
            entity.Property(x => x.EndedAtUtc);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();

            entity.HasOne<Reservation.Domain.Reservation>()
                .WithMany()
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reservation.Domain.OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired();
            entity.Property(x => x.Type).IsRequired().HasMaxLength(200);
            entity.Property(x => x.RoutingKey).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PayloadJson).IsRequired();
            entity.Property(x => x.CorrelationId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.OccurredAtUtc).IsRequired();
            entity.Property(x => x.ProcessedAtUtc);
            entity.Property(x => x.PublishAttempts).HasDefaultValue(0);
            entity.Property(x => x.LastError);
        });
    }
}
