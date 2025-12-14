# Phase 4 – Task 03: Messaging Library (Publisher/Consumer Abstractions)

## Task Type
Platform / Hardening

## Context
We will publish events from StationService and ReservationService.
We need a consistent abstraction so:
- publishers are easy
- consumers are reliable
- telemetry/logging/correlation is consistent
- future services can reuse the same building blocks

## Objective
Implement a small messaging abstraction library for RabbitMQ in .NET.

## Deliverables
Create a reusable library (can live per service initially, but preferably shared):
- Interfaces:
  - `IEventPublisher`
  - `IEventConsumer` or hosted service base
- Implementation for RabbitMQ:
  - JSON serialization
  - Set correlationId header
  - Set messageId to eventId
  - Persistent delivery mode
- Retry strategy for transient publish failures (basic exponential backoff)
- Connection management:
  - Use connection factory and reuse connections/channels appropriately
- Configuration keys:
  - `RabbitMq:Host`
  - `RabbitMq:Port`
  - `RabbitMq:Username`
  - `RabbitMq:Password`
  - `RabbitMq:Exchange` (default: ev.platform)

## Constraints
- Do not introduce heavy frameworks unless justified.
  - Using `RabbitMQ.Client` directly is acceptable.
  - If using MassTransit, justify the trade-offs and keep configuration explicit.
- Must be Kubernetes friendly:
  - connection settings via env vars/secrets
  - graceful shutdown on SIGTERM

## Acceptance Criteria
- Library can publish a message to `ev.platform` exchange with routing key.
- A minimal consumer can receive and log events from a queue.

## Implementation Steps
1. Decide: Raw RabbitMQ.Client vs MassTransit.
2. Implement publisher.
3. Implement consumer base using `BackgroundService`.
4. Add structured logging and correlation.
5. Provide a sample “console consumer” or integration test.

## Verification
- Publish a test event and confirm it appears in queue.
- Confirm logs show correlationId and eventType.

## Stop Condition
Stop after library successfully publishes and consumes a test event.
