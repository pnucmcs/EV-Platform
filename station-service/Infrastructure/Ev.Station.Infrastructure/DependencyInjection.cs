using Ev.Station.Application.Services;
using Ev.Station.Domain;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ev.Station.Infrastructure.Persistence;

namespace Ev.Station.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStationInfrastructure(this IServiceCollection services, string postgresConnection, RabbitMqOptions rabbitOptions)
    {
        services.AddDbContext<StationDbContext>(opt => opt.UseNpgsql(postgresConnection));
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IStationService, StationService>();
        services.AddSingleton(rabbitOptions);
        return services;
    }
}
