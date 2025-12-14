# Phase 4 – Task 08: Notification Service (Minimal Event Consumer)

## Task Type
Feature / Spike

## Context
To prove the event-driven architecture is working end-to-end, we need a real consumer.
NotificationService will be the first consumer. For now it can:
- log messages
- store to a local table (optional)
- later send emails/SMS/push (Phase 9+)

## Objective
Implement a minimal .NET 8 NotificationService that consumes events from RabbitMQ queue:
- `notification-service.events`

## Deliverables
- New microservice repository:
  - NotificationService.Api (optional) OR Worker Service only
  - NotificationService.Application/Domain/Infrastructure (keep it simple; worker is fine)
- RabbitMQ consumer hosted service
- Event handler registry:
  - Handles station.* and reservation/session.* routing keys (at least logs)
- Basic deduplication:
  - store processed eventIds in Postgres (optional but recommended for reliability)
- Kubernetes deployment YAML or Helm chart stub (deployment can wait until Phase 5 if you prefer)

## Constraints
- No real email/SMS integrations yet.
- Must handle malformed messages gracefully.
- Must log correlationId, eventType, eventId.

## Acceptance Criteria
- When Station/Reservation publish events, NotificationService receives and logs them.
- Service remains stable under burst of events.

## Verification
- Deploy NotificationService in Kubernetes
- Trigger a few events
- Show consumer logs

## Stop Condition
Stop after consumer reliably receives and logs events.
