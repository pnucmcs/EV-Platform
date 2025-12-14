using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts;

public sealed class EventEnvelope<TPayload> where TPayload : class, IEvent
{
    [JsonPropertyName("eventId")]
    public Guid EventId { get; init; } = Guid.NewGuid();

    [JsonPropertyName("eventType")]
    public string EventType { get; init; } = string.Empty;

    [JsonPropertyName("eventVersion")]
    public int EventVersion { get; init; }

    [JsonPropertyName("occurredAtUtc")]
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;

    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; init; } = string.Empty;

    [JsonPropertyName("producer")]
    public string Producer { get; init; } = string.Empty; // service name + version

    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; init; } = 1;

    [JsonPropertyName("payload")]
    public TPayload Payload { get; init; }

    public EventEnvelope(TPayload payload, string? correlationId = null, string? producer = null, int? schemaVersion = null)
    {
        Payload = payload;
        EventType = payload.EventType;
        EventVersion = payload.EventVersion;
        CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;
        Producer = producer ?? "unknown";
        SchemaVersion = schemaVersion ?? 1;
    }
}
