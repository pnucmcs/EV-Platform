using Ev.AdminConfig.Domain;
using Ev.AdminConfig.Infrastructure.Persistence;
using Ev.AdminConfig.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.AdminConfig.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminConfigInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        services.AddDbContext<AdminConfigDbContext>(options => options.UseNpgsql(connString));
        services.AddScoped<IAdminConfigRepository, AdminConfigRepository>();
        return services;
    }
}
