using Ev.Telemetry.Domain;
using Ev.Telemetry.Infrastructure.Persistence;
using Ev.Telemetry.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Telemetry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTelemetryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        services.AddDbContext<TelemetryDbContext>(options => options.UseNpgsql(connString));
        services.AddScoped<ITelemetryRepository, TelemetryRepository>();
        return services;
    }
}
