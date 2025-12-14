# Phase 6 – Task 09: Reporting & Analytics MVP

## Task Type
Feature / Reporting

## Context
Reporting is valuable for demonstration and operational insight.
In microservices, reporting often becomes event-driven or read-model driven.

## Objective
Implement a minimal ReportingAnalyticsService that builds a read model from events.

## Deliverables
- ReportingAnalyticsService scaffold with Postgres storage
- Consumer for:
  - station.created/status_changed
  - reservation.created/cancelled
  - session.started/completed
  - invoice.created (if emitted)
- Read models:
  - StationUtilizationDaily (stationId, date, sessionsCount, totalKwh, totalRevenue)
- API:
  - `GET /api/v1/reports/stations/{stationId}/daily?from=...&to=...`

## Constraints
- Eventual consistency is acceptable.
- Ensure consumer is idempotent.

## Acceptance Criteria
- After running simulator and sessions, report endpoint returns meaningful data.
- Service deploys via Helm.

## Stop Condition
Stop after a basic daily utilization report works.
