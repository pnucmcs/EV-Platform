namespace Ev.Platform.Contracts;

public static class EventRoutingKeys
{
    public const string StationCreatedV1 = "station.created.v1";
    public const string StationStatusChangedV1 = "station.status_changed.v1";
    public const string ReservationCreatedV1 = "reservation.created.v1";
    public const string ReservationCancelledV1 = "reservation.cancelled.v1";
    public const string ChargingSessionStartedV1 = "session.started.v1";
    public const string ChargingSessionCompletedV1 = "session.completed.v1";
}
