# Phase 6 – Task 04: TelemetryService (Scaffold + Ingestion + Storage)

## Task Type
Feature

## Context
TelemetryService ingests device readings. For Phase 6, we implement a minimal ingestion API and storage
that later supports session enrichment and monitoring.

## Objective
Implement TelemetryService with:
- ingestion API
- storage (choose one):
  - Option A: PostgreSQL + TimescaleDB style schema (simple)
  - Option B: MongoDB collection for telemetry events (acceptable for MVP)
- readiness for producing telemetry events

## Deliverables
1. Solution and layering:
   - TelemetryService.Api / Application / Domain / Infrastructure
2. Domain model:
   - TelemetryReading (id, deviceId, stationId, timestampUtc, voltage, powerKw, energyKwhDelta, temperatureC, status)
3. API:
   - `POST /api/v1/telemetry/readings` (bulk allowed)
   - `GET /api/v1/telemetry/stations/{stationId}/latest`
4. Validation + mapping + ProblemDetails
5. Helm chart

## Constraints
- Keep payload compact; avoid huge schemas.
- Ensure timestamps are UTC.
- Do not attempt “stream processing” yet.

## Acceptance Criteria
- Ingestion persists data.
- Latest endpoint returns last known telemetry per station/device.
- Service deploys via Helm.

## Stop Condition
Stop after ingestion and latest query work in cluster.
