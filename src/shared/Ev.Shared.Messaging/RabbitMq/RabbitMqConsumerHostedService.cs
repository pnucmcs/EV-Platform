using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ev.Shared.Messaging.RabbitMq;

public interface IRabbitMqMessageHandler
{
    bool CanHandle(string routingKey);
    Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
}

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumerHostedService> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _queue;
    private readonly IEnumerable<string> _bindings;

    public RabbitMqConsumerHostedService(RabbitMqOptions options, IServiceProvider serviceProvider, ILogger<RabbitMqConsumerHostedService> logger, IEnumerable<string> bindings)
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _bindings = bindings;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        _queue = _channel.QueueDeclare().QueueName;
        foreach (var binding in _bindings)
        {
            _channel.QueueBind(_queue, _options.Exchange, binding);
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;
        _channel.BasicConsume(_queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IRabbitMqMessageHandler>();
        var handled = false;
        foreach (var handler in handlers)
        {
            if (handler.CanHandle(ea.RoutingKey))
            {
                await handler.HandleAsync(ea.RoutingKey, ea.Body, CancellationToken.None);
                handled = true;
            }
        }
        _channel?.BasicAck(ea.DeliveryTag, multiple: false);
        if (!handled)
        {
            _logger.LogWarning("No handler for routing key {RoutingKey}", ea.RoutingKey);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
