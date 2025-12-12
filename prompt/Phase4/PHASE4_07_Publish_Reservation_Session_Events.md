# Phase 4 – Task 07: Publish Reservation & Session Events

## Task Type
Feature

## Context
ReservationService owns reservation/session lifecycle. Events enable downstream workflows:
- Notifications
- Billing
- Telemetry correlation
- Reporting

## Objective
Publish these events via Outbox:
- ReservationCreatedV1 -> `reservation.created.v1`
- ReservationCancelledV1 -> `reservation.cancelled.v1`
- ChargingSessionStartedV1 -> `session.started.v1`
- ChargingSessionCompletedV1 -> `session.completed.v1`

## Deliverables
- Event creation using `EvPlatform.Contracts`
- Outbox enqueue logic in command handlers
- Correct routing keys
- CorrelationId included

## Constraints
- Do not include sensitive data.
- Payload should include identifiers and times:
  - reservationId, sessionId, stationId, userId
  - start/end timestamps
  - (optional) energyKwh and costEstimate if available

## Acceptance Criteria
- Each state change results in a published message.
- Messages route to queues per bindings.

## Verification
- Trigger flows via Swagger:
  - create reservation
  - cancel reservation
  - start session
  - stop session
- Confirm messages received.

## Stop Condition
Stop after all four events are published successfully.
