using System.Text;
using System.Text.Json;
using Ev.Platform.Contracts;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Notification.Api.Consumers;

public sealed class NotificationEventHandler : IRabbitMqMessageHandler
{
    private readonly ILogger<NotificationEventHandler> _logger;

    public NotificationEventHandler(ILogger<NotificationEventHandler> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(string routingKey) => routingKey.StartsWith("station.") || routingKey.StartsWith("reservation.") || routingKey.StartsWith("session.");

    public async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, IBasicProperties properties, CancellationToken cancellationToken)
    {
        try
        {
            var json = Encoding.UTF8.GetString(body.Span);
            string? eventId = properties.MessageId;
            string? correlationId = properties.CorrelationId;
            string? eventType = properties.Type ?? routingKey;

            // Try to get occurredAtUtc from envelope for logging context
            using var doc = JsonDocument.Parse(json);
            var occurredAt = doc.RootElement.GetPropertyOrDefault("occurredAtUtc");
            var payloadPreview = doc.RootElement.GetPropertyOrDefault("payload");

            _logger.LogInformation("Notification received eventType={EventType} routingKey={RoutingKey} eventId={EventId} correlationId={CorrelationId} occurredAtUtc={OccurredAt} payload={Payload}",
                eventType,
                routingKey,
                eventId,
                correlationId,
                occurredAt ?? "n/a",
                payloadPreview ?? "n/a");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle event routingKey={RoutingKey} correlationId={CorrelationId}", routingKey, properties.CorrelationId);
        }

        await Task.CompletedTask;
    }
}

internal static class JsonElementExtensions
{
    public static string? GetPropertyOrDefault(this JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value))
        {
            return value.ToString();
        }
        return null;
    }
}
