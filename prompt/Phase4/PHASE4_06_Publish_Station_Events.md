# Phase 4 – Task 06: Publish Station Events (StationCreated, StationStatusChanged)

## Task Type
Feature

## Context
We now have event contracts, RabbitMQ topology, and Outbox dispatch.
StationService must publish domain events so downstream consumers (Notification, Reporting) can react.

## Objective
Publish these events from StationService via Outbox:
- StationCreatedV1
- StationStatusChangedV1

## Deliverables
- Events created using `EvPlatform.Contracts`
- Routing keys:
  - `station.created.v1`
  - `station.status_changed.v1`
- Outbox enqueue on:
  - CreateStationCommand
  - SetStationStatusCommand
- Include correlationId propagation

## Constraints
- Event payload must not include internal DB details.
- Keep payload minimal but useful:
  - stationId
  - name
  - status
  - coordinates
  - occurredAtUtc

## Acceptance Criteria
- Creating station emits event to `ev.platform` exchange.
- Updating status emits event.
- Messages land in `notification-service.events` queue (and any other bound queue).

## Verification
- Use RabbitMQ UI to verify messages in queues.
- Show logs from outbox dispatcher.

## Stop Condition
Stop after events publish and appear in bound queues.
