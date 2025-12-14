using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ev.Reservation.Infrastructure;

public sealed class ReservationDbContextFactory : IDesignTimeDbContextFactory<ReservationDbContext>
{
    public ReservationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReservationDbContext>();
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
                  ?? "Host=localhost;Port=5432;Database=reservation_db;Username=admin;Password=admin";
        optionsBuilder.UseNpgsql(conn);
        return new ReservationDbContext(optionsBuilder.Options);
    }
}
