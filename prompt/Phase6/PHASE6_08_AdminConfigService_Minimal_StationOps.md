# Phase 6 – Task 08: AdminConfigService (Minimal Station Ops + Config)

## Task Type
Feature

## Context
Operators need a controlled surface to manage station configuration and operational toggles
without mixing operator concepts into core StationService indefinitely.

## Objective
Create AdminConfigService with minimal operator APIs:
- maintenance windows
- station operational toggles
- feature flags (optional)

## Deliverables
- AdminConfigService scaffold with Postgres storage
- Domain:
  - MaintenanceWindow (stationId, startUtc, endUtc, reason)
  - StationOpsConfig (stationId, allowReservations bool, allowCharging bool)
- APIs:
  - CRUD maintenance windows
  - Update station ops config
- Event publishing:
  - `station.ops_config_changed.v1` (optional)

## Constraints
- Keep it minimal; no UI.
- Must not duplicate StationService responsibilities.

## Acceptance Criteria
- Operator config persists and can be queried.
- Future services can consume ops config (document how).

## Stop Condition
Stop after AdminConfigService deploys and APIs work.
