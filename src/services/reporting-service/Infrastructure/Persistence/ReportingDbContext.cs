using Ev.Reporting.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Reporting.Infrastructure.Persistence;

public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
    {
    }

    public DbSet<StationUtilizationDaily> StationUtilizations => Set<StationUtilizationDaily>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StationUtilizationDaily>(entity =>
        {
            entity.ToTable("station_utilization_daily");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StationId).IsRequired();
            entity.Property(x => x.Date).IsRequired();
            entity.Property(x => x.SessionsCount).IsRequired();
            entity.Property(x => x.TotalKwh).IsRequired();
            entity.Property(x => x.TotalRevenue).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.StationId, x.Date }).IsUnique();
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.ToTable("processed_events");
            entity.HasKey(x => x.EventId);
            entity.Property(x => x.ProcessedAtUtc).IsRequired();
        });
    }
}
