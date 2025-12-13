# Eventing Standards – EV Platform

## Naming conventions
- Exchange: `ev.platform` (topic)
- Routing keys: `<domain>.<event>.<version>` (e.g., `station.created.v1`, `reservation.created.v1`)
- Queue names: `<consumer-service>.<event>` (per consumer; e.g., `notification.reservation-created`)

## Envelope
Required fields (JSON):
- `eventId` (guid)
- `eventType` (routing key)
- `eventVersion` (int)
- `occurredAtUtc` (ISO-8601 UTC)
- `correlationId`
- `producer` (service name + version)
- `schemaVersion`
- `payload` (event body)

## Contracts (v1)
Defined in `src/shared/Ev.Platform.Contracts`:
- `StationCreatedV1`
- `StationStatusChangedV1`
- `ReservationCreatedV1`
- `ReservationCancelledV1`
- `ChargingSessionStartedV1`
- `ChargingSessionCompletedV1`

Routing keys:
- `station.created.v1`
- `station.status_changed.v1`
- `reservation.created.v1`
- `reservation.cancelled.v1`
- `charging-session.started.v1`
- `charging-session.completed.v1`

## Examples

`station.created.v1`
```json
{
  "eventId": "7e5f3c46-7f57-4df1-92a5-6ac3a8fe6d11",
    "eventType": "station.created.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2024-01-01T12:00:00Z",
  "correlationId": "c0f6e6d7-a8c4-4f6c-9f0f-8c9a7d8e6a1b",
  "producer": "station-service@1.0.0",
  "schemaVersion": 1,
  "payload": {
    "stationId": "f1c5f0a3-3a5e-4ed6-bab3-23d4c9c0e5f7",
    "name": "Main St",
    "latitude": 40.0,
    "longitude": -73.9,
    "status": "Online"
  }
}
```

`reservation.created.v1`
```json
{
  "eventId": "e7f4b8b2-1a55-4c7a-8d3e-7c5a1b2c3d4e",
    "eventType": "reservation.created.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2024-01-01T12:05:00Z",
  "correlationId": "c0f6e6d7-a8c4-4f6c-9f0f-8c9a7d8e6a1b",
  "producer": "reservation-service@1.0.0",
  "schemaVersion": 1,
  "payload": {
    "reservationId": "3a1f9c6d-4b7e-4a3a-9e6c-1b2c3d4e5f6a",
    "stationId": "f1c5f0a3-3a5e-4ed6-bab3-23d4c9c0e5f7",
    "userId": "c2d3e4f5-6789-4abc-def0-1234567890ab",
    "startsAtUtc": "2024-01-02T09:00:00Z",
    "endsAtUtc": "2024-01-02T10:00:00Z",
    "status": "Created"
  }
}
```

`charging-session.completed.v1`
```json
{
  "eventId": "c5d6e7f8-1234-4abc-9def-567890abcdef",
  "eventType": "charging-session.completed.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2024-01-02T10:15:00Z",
  "correlationId": "c0f6e6d7-a8c4-4f6c-9f0f-8c9a7d8e6a1b",
  "producer": "reservation-service@1.0.0",
  "schemaVersion": 1,
  "payload": {
    "sessionId": "8a7b6c5d-4e3f-2a1b-9c0d-ef1234567890",
    "reservationId": "3a1f9c6d-4b7e-4a3a-9e6c-1b2c3d4e5f6a",
    "stationId": "f1c5f0a3-3a5e-4ed6-bab3-23d4c9c0e5f7",
    "chargerId": "b4c3d2e1-f0a9-8b7c-6d5e-4f3a2b1c0d9e",
    "startedAtUtc": "2024-01-02T09:00:00Z",
    "endedAtUtc": "2024-01-02T10:15:00Z",
    "energyKWh": 22.5
  }
}
```

## Versioning guidance
- v1 contracts are stable; new fields should be additive and optional.
- Breaking changes require a new routing key/version (e.g., `station.created.v2`) and side-by-side consumers.
- Keep schemaVersion aligned with envelope schema changes; payload changes are managed via eventVersion/routing key.

## Queue guidelines
- Each consumer owns its queue(s); do not share queues between services.
- Use auto-delete=false, durable=true queues for persistence.
***/
