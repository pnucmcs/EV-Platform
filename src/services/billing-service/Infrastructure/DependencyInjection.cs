using Ev.Billing.Domain;
using Ev.Billing.Infrastructure.Persistence;
using Ev.Billing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        services.AddDbContext<BillingDbContext>(options => options.UseNpgsql(connString));
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        return services;
    }
}
