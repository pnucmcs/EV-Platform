# Phase 3 – Task 02: Dockerfile + Container Image (StationService)

## Task Type
Feature / Platform Enablement

## Context
StationService must run as a container in Kubernetes. We will build a production-grade image:
- multi-stage build
- non-root runtime
- deterministic port
- health-friendly defaults

## Objective
Create a Docker build for StationService that is Kubernetes-ready and uses env vars for configuration.

## Deliverables
- `StationService.Api/Dockerfile` (or repository root Dockerfile; choose one approach and document)
- `.dockerignore`
- `README.md` section: “Build and run container locally”
- Container exposes port `8080`
- Container uses non-root user (where feasible with base image)
- Runtime image based on official Microsoft .NET 8 ASP.NET runtime

## Constraints
- No secrets baked into image.
- Do not add docker-compose for infra; TrueNAS services already exist.
- Must support `ASPNETCORE_ENVIRONMENT=Production`.

## Acceptance Criteria
- `docker build` succeeds
- `docker run -p 8080:8080 ...` starts API
- `/health/live` returns 200
- If Swagger is enabled only in Development, confirm it is OFF in Production

## Implementation Steps
1. Add `.dockerignore` (bin/obj, .git, etc.)
2. Create multi-stage Dockerfile:
   - Stage 1: restore
   - Stage 2: build
   - Stage 3: publish
   - Stage 4: runtime
3. Set:
   - `ASPNETCORE_URLS=http://0.0.0.0:8080` OR rely on Kestrel env config from Task 01
4. Add minimal runtime hardening:
   - `USER` non-root (if possible)
   - `WORKDIR /app`
5. Provide run example using your Postgres:
   - `ConnectionStrings__Postgres=Host=192.168.1.191;Port=5432;Database=...;Username=...;Password=...`

## Verification
- Build image
- Run container
- Hit `/health/live` and (if implemented) `/health/ready`

## Stop Condition
Stop after StationService container runs and responds on port 8080.
