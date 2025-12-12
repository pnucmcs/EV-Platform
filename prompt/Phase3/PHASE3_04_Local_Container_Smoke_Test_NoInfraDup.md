# Phase 3 – Task 04: Local Container Smoke Test (No Infra Duplication)

## Task Type
Spike / Validation

## Context
We must validate containers can:
- connect to TrueNAS-hosted PostgreSQL (`192.168.1.191:5432`)
- communicate service-to-service (Reservation → Station)
without spinning up duplicate infra locally.

## Objective
Provide repeatable scripts to run both containers locally and validate core flows end-to-end.

## Deliverables
In a `/scripts` folder (in each repo or a shared ops repo):
- `run_station_container.sh`
- `run_reservation_container.sh`
- `smoke_test.sh` (optional but recommended)
- `.env.station.example`
- `.env.reservation.example`

Scripts must:
- set required environment variables
- start containers with deterministic ports:
  - StationService -> host port 8080
  - ReservationService -> host port 8081

## Constraints
- Do NOT start Postgres/Mongo/Redis containers.
- Use your TrueNAS endpoints for dependencies.
- Do NOT require manual editing beyond filling credentials.

## Acceptance Criteria
- One command starts each service container
- Smoke test executes:
  1) Create station
  2) Create reservation for that station
  3) Start session
  4) Stop session
- Exit code 0 on success, non-zero on failure

## Implementation Steps
1. Write env example files with placeholders:
   - `ConnectionStrings__Postgres=Host=192.168.1.191;Port=5432;Database=...;Username=...;Password=...`
2. Run Station container
3. Run Reservation container with:
   - `Services__StationService__BaseUrl=http://host.docker.internal:8080` (or explicit host IP)
4. Implement smoke script using `curl` (no extra tooling required)

## Verification
- Run scripts and attach console output.
- Confirm both services remain healthy (`/health/live`).

## Stop Condition
Stop after scripted smoke test passes reliably.
