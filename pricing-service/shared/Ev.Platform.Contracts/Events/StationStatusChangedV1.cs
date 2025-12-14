using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class StationStatusChangedV1 : IEvent
{
    [JsonPropertyName("stationId")]
    public Guid StationId { get; init; }

    [JsonPropertyName("previousStatus")]
    public string PreviousStatus { get; init; } = string.Empty;

    [JsonPropertyName("newStatus")]
    public string NewStatus { get; init; } = string.Empty;

    [JsonPropertyName("occurredAtUtc")]
    public DateTime OccurredAtUtc { get; init; }

    public string EventType => EventRoutingKeys.StationStatusChangedV1;
    public int EventVersion => 1;
}
