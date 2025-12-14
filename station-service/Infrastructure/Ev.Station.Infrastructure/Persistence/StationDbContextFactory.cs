using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ev.Station.Infrastructure.Persistence;

public sealed class StationDbContextFactory : IDesignTimeDbContextFactory<StationDbContext>
{
    public StationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StationDbContext>();
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
                  ?? "Host=localhost;Port=5432;Database=station_db;Username=admin;Password=admin";
        optionsBuilder.UseNpgsql(conn);
        return new StationDbContext(optionsBuilder.Options);
    }
}
