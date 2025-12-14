using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class ChargingSessionCompletedV1 : IEvent
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

    [JsonPropertyName("endedAtUtc")]
    public DateTime EndedAtUtc { get; init; }

    [JsonPropertyName("energyKWh")]
    public double? EnergyKWh { get; init; }

    public string EventType => EventRoutingKeys.ChargingSessionCompletedV1;
    public int EventVersion => 1;
}
