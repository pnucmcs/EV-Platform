using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class ReservationCreatedV1 : IEvent
{
    [JsonPropertyName("reservationId")]
    public Guid ReservationId { get; init; }

    [JsonPropertyName("stationId")]
    public Guid StationId { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("startsAtUtc")]
    public DateTime StartsAtUtc { get; init; }

    [JsonPropertyName("endsAtUtc")]
    public DateTime EndsAtUtc { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    public string EventType => EventRoutingKeys.ReservationCreatedV1;
    public int EventVersion => 1;
}
