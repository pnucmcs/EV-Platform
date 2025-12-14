# Phase 3 – Task 03: Dockerfile + Container Image (ReservationService)

## Task Type
Feature / Platform Enablement

## Context
ReservationService must run as a container in Kubernetes and call StationService via configuration.

## Objective
Create a Docker build for ReservationService that is Kubernetes-ready and uses env vars for:
- Postgres connection
- StationService base URL

## Deliverables
- `ReservationService.Api/Dockerfile`
- `.dockerignore` (if not already shared)
- `README.md` section: “Build and run container locally”
- Container exposes port `8080`

## Constraints
- No secrets baked into image.
- No infra duplication (no local Postgres/Mongo/Redis in compose).
- Must support `Services:StationService:BaseUrl` via env var.

## Acceptance Criteria
- `docker build` succeeds
- `docker run -p 8081:8080 ...` starts API
- `/health/live` returns 200
- Reservation creation fails gracefully if StationService URL is missing/unreachable

## Implementation Steps
1. Create multi-stage Dockerfile (same pattern as StationService for consistency).
2. Ensure config keys match Task 01:
   - `ConnectionStrings__Postgres`
   - `Services__StationService__BaseUrl`
3. Provide run example that targets StationService container:
   - StationService run on `http://host.docker.internal:8080` OR a shared docker network alias

## Verification
- Run StationService container
- Run ReservationService container
- Create station, then create reservation referencing that station

## Stop Condition
Stop after ReservationService container can run and validate StationId via HTTP.
