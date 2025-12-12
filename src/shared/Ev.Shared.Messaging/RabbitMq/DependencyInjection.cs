using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ev.Shared.Messaging.RabbitMq;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, Action<RabbitMqOptions> configure)
    {
        var options = new RabbitMqOptions();
        configure(options);
        services.AddSingleton(options);
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        return services;
    }

    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, Action<RabbitMqOptions> configure, params string[] bindings)
    {
        var options = new RabbitMqOptions();
        configure(options);
        services.AddSingleton(options);
        services.AddHostedService(sp =>
            new RabbitMqConsumerHostedService(options, sp, sp.GetRequiredService<ILogger<RabbitMqConsumerHostedService>>(), bindings));
        return services;
    }
}
