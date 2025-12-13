# Phase 6 – Service Expansion Plan (Dependency Map + Scope Locks)

## Current baseline
- StationService: owns stations/chargers; HTTP + events via outbox (`station.*.v1`).
- ReservationService: owns reservations/sessions; HTTP + events via outbox (`reservation.*.v1`, `session.*.v1`).
- NotificationService: consumer of station/reservation/session events; logs only.

## Dependency map (HTTP vs Events)
### PricingService
- Purpose: compute dynamic pricing per station/connector/time/rate-plan.
- Owned store: Postgres (pricing rules, rate plans).
- HTTP: expose `GET /pricing/quote?stationId&connectorType&kWh` for synchronous pricing.
- Events consumed: `station.created.v1`, `station.status_changed.v1` (warm cache); optional `reservation.created.v1`.
- Events emitted: `price.quoted.v1` (optional later) or `pricing.rule.updated.v1`.

### BillingService
- Purpose: calculate charges and produce invoices for completed sessions.
- Owned store: Postgres (invoices, payments ledger); defer real payment gateway.
- HTTP: `POST /billing/invoices/{sessionId}/finalize` (internal) or background worker.
- Events consumed: `session.completed.v1`, `session.started.v1` (for provisional holds).
- Events emitted: `invoice.generated.v1`, `payment.failed.v1` (later), `payment.succeeded.v1` (later).

### TelemetryService
- Purpose: ingest charger telemetry/metrics for health and reporting.
- Owned store: Time-series DB or Postgres (MVP Postgres).
- HTTP: `POST /telemetry/events` (ingest); `GET /telemetry/stations/{id}/latest`.
- Events consumed: optional `station.status_changed.v1` for state alignment.
- Events emitted: `telemetry.ingested.v1`, `charger.health_changed.v1` (later).

### AdminConfigService
- Purpose: manage feature flags, station/operator config, rate-plan assignments.
- Owned store: Postgres (config + history).
- HTTP: `GET/PUT /admin/config/{key}`; `PUT /admin/stations/{id}/settings`.
- Events consumed: `station.created.v1` (seed config), `station.status_changed.v1`.
- Events emitted: `config.changed.v1`, `station.config.updated.v1` (optional).

### ReportingAnalyticsService
- Purpose: aggregate cross-domain data for dashboards/exports.
- Owned store: Postgres/ClickHouse (MVP Postgres).
- HTTP: `GET /reports/stations`, `GET /reports/revenue`, `GET /reports/utilization`.
- Events consumed: `station.*.v1`, `reservation.*.v1`, `session.*.v1`, `invoice.generated.v1`.
- Events emitted: none (read model only).

### Auth/Identity integration (later)
- Purpose: issue/validate access tokens, user identities.
- Owned store: identity provider (external or later self-hosted).
- HTTP: integrate via JWT validation middleware; user lookup if needed.
- Events consumed: user lifecycle events (later).
- Events emitted: `user.created.v1` (future).

## Scope locks (Phase 6 MVP vs deferred)
- **Implement now (MVP)**:
  - PricingService: basic rule lookup; synchronous quote API; consumes station events.
  - BillingService: compute invoice on `session.completed.v1`; emit `invoice.generated.v1`; no real payments.
  - ReportingAnalyticsService: consume existing events and surface minimal read endpoints.
  - Helm/k8s scaffolding for each new service; OTEL/metrics baseline; RabbitMQ bindings.
- **Deferred explicitly**:
  - Real payment gateway / PCI workflows (stripe/etc.).
  - External identity provider integration (use static API key/JWT dev signing for now).
  - Rich telemetry/time-series backend (stick to Postgres in MVP).
  - Complex pricing (tiered/TOU) beyond simple rule table.

## Updated event catalog (new)
- `price.quoted.v1` (optional; correlates quote to reservation/session).
- `pricing.rule.updated.v1`
- `invoice.generated.v1`
- `payment.failed.v1` / `payment.succeeded.v1` (deferred)
- `telemetry.ingested.v1`
- `charger.health_changed.v1` (later)
- `config.changed.v1`
- `station.config.updated.v1` (optional)

## Implementation order (backlog)
1) **PricingService MVP**: contracts, API, Postgres schema, consumer for station events.  
2) **BillingService MVP**: consume `session.completed.v1`, compute invoice, emit `invoice.generated.v1`; outbox + RabbitMQ bindings.  
3) **ReportingAnalyticsService**: consumer for existing events, build minimal read endpoints, dashboards tie-in.  
4) **TelemetryService MVP**: ingest endpoint + minimal events; optional consumer hooks.  
5) **AdminConfigService**: CRUD + events for config changes.  
6) **Auth/Identity integration spike**: JWT validation plan; defer full implementation.

## Notes on compatibility
- Keep all new events at `v1`; if payload changes, version with suffix `.v2` and keep routing key stable (`event.v2`).
- Use existing envelope (`EventEnvelope<T>`) and outbox pattern; keep `routing_key` aligned to catalog.
