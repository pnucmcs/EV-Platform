using Ev.Reporting.Domain.Repositories;
using Ev.Reporting.Infrastructure.Persistence;
using Ev.Reporting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Reporting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        services.AddDbContext<ReportingDbContext>(options => options.UseNpgsql(connString));
        services.AddScoped<IReportingRepository, ReportingRepository>();
        return services;
    }
}
