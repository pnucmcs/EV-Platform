using Ev.Notification.Domain;
using Ev.Notification.Infrastructure.Persistence;
using Ev.Notification.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Postgres") ?? configuration["ConnectionStrings:Postgres"] ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        services.AddDbContext<NotificationDbContext>(options => options.UseNpgsql(connString));
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }
}
