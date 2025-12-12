# Phase 4 – Task 05: Outbox Pattern (ReservationService)

## Task Type
Hardening / Reliability

## Context
ReservationService will publish:
- ReservationCreatedV1
- ReservationCancelledV1
- ChargingSessionStartedV1
- ChargingSessionCompletedV1

Reliability matters even in a lab environment if this is to be portfolio-grade.

## Objective
Implement an Outbox pattern in ReservationService (same approach as StationService).

## Deliverables
- `outbox_messages` table + migration
- Enqueue events in same transaction as state changes
- Background dispatcher to publish to RabbitMQ exchange `ev.platform`
- Routing keys aligned to Phase 4 Task 01 standard

## Constraints
- Consistent schema and behavior with StationService outbox
- Idempotency via eventId messageId

## Acceptance Criteria
- Reservation create generates an outbox message.
- Session complete generates an outbox message.
- Dispatcher publishes and marks processed.
- RabbitMQ downtime does not lose events.

## Verification
- Repeat the RabbitMQ down/up test.
- Confirm routing keys match bindings.

## Stop Condition
Stop after outbox dispatch works reliably for key flows.
