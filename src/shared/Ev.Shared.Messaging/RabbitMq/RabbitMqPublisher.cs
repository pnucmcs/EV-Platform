using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Ev.Shared.Messaging.RabbitMq;

public interface IRabbitMqPublisher
{
    void Publish<T>(string routingKey, T message);
}

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(RabbitMqOptions options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public void Publish<T>(string routingKey, T message)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        _channel.BasicPublish(exchange: _options.Exchange, routingKey: routingKey, basicProperties: props, body: payload);
        _logger.LogInformation("Published event {RoutingKey}", routingKey);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
