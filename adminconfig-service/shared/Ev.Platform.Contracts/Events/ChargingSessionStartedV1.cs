using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class ChargingSessionStartedV1 : IEvent
{
    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; init; }

    [JsonPropertyName("reservationId")]
    public Guid ReservationId { get; init; }

    [JsonPropertyName("stationId")]
    public Guid StationId { get; init; }

    [JsonPropertyName("chargerId")]
    public Guid? ChargerId { get; init; }

    [JsonPropertyName("startedAtUtc")]
    public DateTime StartedAtUtc { get; init; }

    public string EventType => EventRoutingKeys.ChargingSessionStartedV1;
    public int EventVersion => 1;
}
