using Ev.Pricing.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Pricing.Infrastructure.Persistence;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
    {
    }

    public DbSet<TariffPlan> TariffPlans => Set<TariffPlan>();
    public DbSet<TimeOfUseRule> TimeOfUseRules => Set<TimeOfUseRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TariffPlan>(entity =>
        {
            entity.ToTable("tariff_plans");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            entity.Property(x => x.BaseRatePerKwh).HasColumnType("numeric(18,4)");
            entity.Property(x => x.IdleFeePerMinute).HasColumnType("numeric(18,4)");
            entity.Property(x => x.ValidFromUtc).IsRequired();
            entity.Property(x => x.ValidToUtc);
            entity.HasMany(x => x.TimeOfUseRules)
                .WithOne()
                .HasForeignKey(r => r.TariffPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TimeOfUseRule>(entity =>
        {
            entity.ToTable("time_of_use_rules");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DayOfWeek).IsRequired();
            entity.Property(x => x.StartTime).IsRequired();
            entity.Property(x => x.EndTime).IsRequired();
            entity.Property(x => x.Multiplier).HasColumnType("numeric(9,4)").HasDefaultValue(1m);
        });
    }
}
