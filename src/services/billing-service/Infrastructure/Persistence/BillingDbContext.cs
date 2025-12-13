using Ev.Billing.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ev.Billing.Infrastructure.Persistence;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Amount).HasColumnType("numeric(18,4)");
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasMany(x => x.PaymentAttempts)
                .WithOne()
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.SessionId).IsUnique();
        });

        modelBuilder.Entity<PaymentAttempt>(entity =>
        {
            entity.ToTable("payment_attempts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.Provider).HasMaxLength(50).HasDefaultValue("stub");
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.LastError);
        });
    }
}
