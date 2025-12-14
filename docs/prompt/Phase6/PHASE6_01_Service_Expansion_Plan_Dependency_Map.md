# Phase 6 – Task 01: Service Expansion Plan (Dependency Map + Scope Locks)

## Task Type
Architecture / Planning

## Context
We currently have:
- StationService (HTTP + events via outbox)
- ReservationService (HTTP + events via outbox)
- NotificationService (minimal consumer)

Phase 6 expands the platform with additional domain services while preserving:
- consistent contracts
- repeatable deployment pattern (Helm)
- event-driven workflows

## Objective
Create a dependency map and a scoped implementation sequence for the remaining services.

## Deliverables
1. Dependency map (HTTP vs Events):
   - PricingService
   - BillingService
   - TelemetryService
   - AdminConfigService
   - ReportingAnalyticsService
   - Auth/Identity integration plan (later if not now)
2. Scope locks for Phase 6:
   - What we will implement now (MVP)
   - What is explicitly deferred (e.g., real payment gateway)
3. Updated event catalog:
   - which new events will be emitted/consumed

## Constraints
- Do not expand scope into UI or real payment processing yet.
- Keep contracts backward compatible; version events if needed.

## Acceptance Criteria
- A clear ordered backlog (next tasks) is produced and agreed.
- Each new service has:
  - purpose
  - owned data store
  - APIs
  - events consumed/emitted

## Stop Condition
Stop after the dependency map and implementation order are finalized.
