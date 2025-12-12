using Ev.Station.Application.Services;
using Ev.Station.Domain;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Ev.Station.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStationInfrastructure(this IServiceCollection services, StationMongoSettings mongoSettings, string redisConnection, RabbitMqOptions rabbitOptions)
    {
        services.AddSingleton(mongoSettings);
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IStationService, StationService>();
        services.AddSingleton(rabbitOptions);
        return services;
    }
}
