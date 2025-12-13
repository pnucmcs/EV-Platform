using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Ev.Platform.Contracts;

namespace Ev.Shared.Messaging.RabbitMq;

public interface IEventPublisher
{
    Task PublishAsync<T>(string routingKey, EventEnvelope<T> envelope, CancellationToken cancellationToken = default) where T : class, IEvent;
}

public interface IRabbitMqPublisher : IEventPublisher
{
    [Obsolete("Use PublishAsync with EventEnvelope<T>.")]
    void Publish<T>(string routingKey, T message) where T : class;

    Task PublishRawAsync(string routingKey, ReadOnlyMemory<byte> body, Guid messageId, string correlationId, string type, CancellationToken cancellationToken = default);
}

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);
    private readonly object _lock = new();

    public RabbitMqPublisher(RabbitMqOptions options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public void Publish<T>(string routingKey, T message) where T : class
    {
        var correlation = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

        if (message is IEvent evt)
        {
            var envelope = new EventEnvelope<IEvent>(evt, correlationId: correlation);
            PublishAsyncInternal(routingKey, JsonSerializer.SerializeToUtf8Bytes(envelope, _serializerOptions), envelope.EventId, envelope.CorrelationId, envelope.EventType, CancellationToken.None)
                .GetAwaiter().GetResult();
            return;
        }

        var wrapper = new WrapperEvent<T>(routingKey, message);
        var wrappedEnvelope = new EventEnvelope<WrapperEvent<T>>(wrapper, correlationId: correlation, producer: "unknown", schemaVersion: 1);
        PublishAsyncInternal(routingKey, JsonSerializer.SerializeToUtf8Bytes(wrappedEnvelope, _serializerOptions), wrappedEnvelope.EventId, wrappedEnvelope.CorrelationId, wrappedEnvelope.EventType, CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    public Task PublishAsync<T>(string routingKey, EventEnvelope<T> envelope, CancellationToken cancellationToken = default) where T : class, IEvent
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(envelope, _serializerOptions);
        return PublishAsyncInternal(routingKey, body, envelope.EventId, envelope.CorrelationId, envelope.EventType, cancellationToken);
    }

    private async Task PublishAsyncInternal(string routingKey, byte[] body, Guid messageId, string correlationId, string type, CancellationToken cancellationToken)
    {
        var props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.MessageId = messageId.ToString();
        props.CorrelationId = correlationId;
        props.Type = type;
        props.DeliveryMode = 2; // persistent

        using var activity = RabbitMqTelemetry.StartPublishActivity(routingKey, type);
        if (Activity.Current is { } current)
        {
            RabbitMqTelemetry.Inject(current, props);
        }

        var delay = TimeSpan.FromMilliseconds(200);
        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                lock (_lock)
                {
                    _channel.BasicPublish(exchange: _options.Exchange, routingKey: routingKey, basicProperties: props, body: body);
                }
                _logger.LogInformation("Published event {RoutingKey} correlationId={CorrelationId} messageId={MessageId}", routingKey, correlationId, messageId);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Failed to publish routingKey {RoutingKey} (attempt {Attempt}/{MaxAttempts}), retrying...", routingKey, attempt, maxAttempts);
                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
        }

        throw new InvalidOperationException($"Failed to publish message after {maxAttempts} attempts for routingKey {routingKey}.");
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

    public Task PublishRawAsync(string routingKey, ReadOnlyMemory<byte> body, Guid messageId, string correlationId, string type, CancellationToken cancellationToken = default)
    {
        return PublishAsyncInternal(routingKey, body.ToArray(), messageId, correlationId, type, cancellationToken);
    }
}

internal sealed class WrapperEvent<T> : IEvent
{
    public WrapperEvent(string routingKey, T payload)
    {
        Payload = payload;
        EventType = routingKey;
    }

    public T Payload { get; }
    public string EventType { get; }
    public int EventVersion => 1;
}
