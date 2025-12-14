# Phase 4 – Task 01: Eventing Standards (Contracts, Naming, Versioning)

## Task Type
Architecture / Hardening

## Context
Phase 4 introduces event-driven architecture using **RabbitMQ (inside Kubernetes)**.
Before publishing any events, we must define a consistent standard so that future services
(Notification, Billing, Telemetry, Reporting) can consume events safely.

## Objective
Define the platform eventing standards and create a versioned contract package/library.

## Deliverables
1. **Event Naming Standard**
   - Exchange naming convention (e.g., `ev.platform`)
   - Routing key convention (e.g., `station.created.v1`, `reservation.created.v1`)
   - Queue naming convention (consumer-specific queues)
2. **Event Envelope Standard**
   - Mandatory headers/fields:
     - eventId (GUID)
     - eventType (string)
     - eventVersion (int)
     - occurredAtUtc (ISO-8601)
     - correlationId (string)
     - producer (service name + version)
     - schema/contract version
3. **Event Contracts Project**
   - Create a shared, versioned package:
     - Option A: `EvPlatform.Contracts` NuGet package (preferred for polyrepo)
     - Option B: Git submodule/shared repo (only if NuGet is too heavy now)
   - Contracts for at least:
     - StationCreatedV1
     - StationStatusChangedV1
     - ReservationCreatedV1
     - ReservationCancelledV1
     - ChargingSessionStartedV1
     - ChargingSessionCompletedV1
4. Documentation:
   - `docs/eventing-standards.md` with examples for each event

## Constraints
- Contracts must be backward compatible (v1 stable).
- No service should depend on another service's internal models.
- Do not include sensitive data in events (no payment details, secrets, PII beyond userId if necessary).
- Keep JSON serialization stable (explicit property names).

## Acceptance Criteria
- Event contract library builds and is referenceable by both StationService and ReservationService.
- At least one example JSON payload per event in docs.

## Implementation Steps
1. Create `EvPlatform.Contracts` project/repo (or folder).
2. Define base envelope and event interfaces.
3. Define event payloads with explicit JSON property names.
4. Add sample JSON and routing keys in docs.
5. Add versioning guidance (how to add v2 later).

## Verification
- Build contracts project.
- Reference it from StationService and ReservationService successfully.

## Stop Condition
Stop after standards + contracts compile and are documented.
