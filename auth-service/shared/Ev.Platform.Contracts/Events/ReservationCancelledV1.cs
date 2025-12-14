using System.Text.Json.Serialization;

namespace Ev.Platform.Contracts.Events;

public sealed class ReservationCancelledV1 : IEvent
{
    [JsonPropertyName("reservationId")]
    public Guid ReservationId { get; init; }

    [JsonPropertyName("stationId")]
    public Guid StationId { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("cancelledAtUtc")]
    public DateTime CancelledAtUtc { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    public string EventType => EventRoutingKeys.ReservationCancelledV1;
    public int EventVersion => 1;
}
