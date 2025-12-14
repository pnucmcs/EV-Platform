using Ev.AdminConfig.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.AdminConfig.Infrastructure.Persistence;

public class AdminConfigDbContext : DbContext
{
    public AdminConfigDbContext(DbContextOptions<AdminConfigDbContext> options) : base(options)
    {
    }

    public DbSet<MaintenanceWindow> MaintenanceWindows => Set<MaintenanceWindow>();
    public DbSet<StationOpsConfig> StationOpsConfigs => Set<StationOpsConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaintenanceWindow>(entity =>
        {
            entity.ToTable("maintenance_windows");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StationId).IsRequired();
            entity.Property(x => x.StartUtc).IsRequired();
            entity.Property(x => x.EndUtc).IsRequired();
            entity.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            entity.HasIndex(x => x.StationId);
        });

        modelBuilder.Entity<StationOpsConfig>(entity =>
        {
            entity.ToTable("station_ops_configs");
            entity.HasKey(x => x.StationId);
            entity.Property(x => x.AllowReservations).IsRequired();
            entity.Property(x => x.AllowCharging).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
        });
    }
}
