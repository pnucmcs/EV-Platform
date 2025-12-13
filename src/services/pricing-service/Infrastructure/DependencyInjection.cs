using Ev.Pricing.Domain;
using Ev.Pricing.Infrastructure.Persistence;
using Ev.Pricing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Pricing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");

        services.AddDbContext<PricingDbContext>(options =>
        {
            options.UseNpgsql(connString);
        });

        services.AddScoped<ITariffPlanRepository, TariffPlanRepository>();

        return services;
    }
}
