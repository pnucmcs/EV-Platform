using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class StationCreatedV1 : IEvent
{
    [JsonPropertyName("stationId")]
    public Guid StationId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = "Unknown";

    public string EventType => EventRoutingKeys.StationCreatedV1;
    public int EventVersion => 1;
}
