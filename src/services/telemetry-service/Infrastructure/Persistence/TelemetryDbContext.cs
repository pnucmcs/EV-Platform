using Ev.Telemetry.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Telemetry.Infrastructure.Persistence;

public class TelemetryDbContext : DbContext
{
    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options)
    {
    }

    public DbSet<TelemetryReading> TelemetryReadings => Set<TelemetryReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryReading>(entity =>
        {
            entity.ToTable("telemetry_readings");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DeviceId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.StationId).IsRequired();
            entity.Property(x => x.TimestampUtc).IsRequired();
            entity.Property(x => x.Voltage).HasColumnType("numeric(18,4)");
            entity.Property(x => x.PowerKw).HasColumnType("numeric(18,4)");
            entity.Property(x => x.EnergyKwhDelta).HasColumnType("numeric(18,4)");
            entity.Property(x => x.TemperatureC).HasColumnType("numeric(18,4)");
            entity.Property(x => x.Status).HasMaxLength(100);
            entity.HasIndex(x => new { x.StationId, x.TimestampUtc });
        });
    }
}
