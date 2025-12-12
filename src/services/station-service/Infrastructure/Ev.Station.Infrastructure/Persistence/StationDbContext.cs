using Ev.Station.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Station.Infrastructure.Persistence;

public sealed class StationDbContext : DbContext
{
    public StationDbContext(DbContextOptions<StationDbContext> options) : base(options)
    {
    }

    public DbSet<Station.Domain.Station> Stations => Set<Station.Domain.Station>();
    public DbSet<Charger> Chargers => Set<Charger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Station.Domain.Station>(entity =>
        {
            entity.ToTable("stations");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Latitude)
                .IsRequired();

            entity.Property(x => x.Longitude)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc);

            entity.HasMany(x => x.Chargers)
                .WithOne()
                .HasForeignKey(c => c.StationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(x => x.Chargers)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Charger>(entity =>
        {
            entity.ToTable("chargers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.StationId)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ConnectorType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc);
        });
    }
}
