using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ev.Shared.Messaging.RabbitMq;

public interface IRabbitMqMessageHandler
{
    bool CanHandle(string routingKey);
    Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, IBasicProperties properties, CancellationToken cancellationToken);
}

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumerHostedService> _logger;
    private readonly IEnumerable<string> _bindings;
    private readonly string _queueName;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumerHostedService(RabbitMqOptions options, IServiceProvider serviceProvider, ILogger<RabbitMqConsumerHostedService> logger, string queueName, IEnumerable<string> bindings)
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _bindings = bindings;
        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(0, _options.PrefetchCount, global: false);

        foreach (var binding in _bindings)
        {
            _channel.QueueBind(_queueName, _options.Exchange, binding);
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;
        _channel.BasicConsume(_queueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        using var activity = RabbitMqTelemetry.StartConsumeActivity(ea);
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IRabbitMqMessageHandler>();
            var handled = false;
            foreach (var handler in handlers)
            {
                if (handler.CanHandle(ea.RoutingKey))
                {
                    await handler.HandleAsync(ea.RoutingKey, ea.Body, ea.BasicProperties, CancellationToken.None);
                    handled = true;
                }
            }
            _channel?.BasicAck(ea.DeliveryTag, multiple: false);
            if (!handled)
            {
                _logger.LogWarning("No handler for routing key {RoutingKey}", ea.RoutingKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message routingKey={RoutingKey} correlationId={CorrelationId}", ea.RoutingKey, ea.BasicProperties?.CorrelationId);
            _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
