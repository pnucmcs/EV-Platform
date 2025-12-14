namespace Ev.Shared.Messaging.Events;

public sealed record StationStatusChanged(Guid StationId, string Status, DateTime OccurredAtUtc);
