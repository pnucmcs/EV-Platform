# Task — Station Service Skeleton (Phase 2 starter)

## Task title
Bootstrap Station Service .NET 8 solution structure

## Task type
Feature

## Context
Station & Parking Service is the foundation for the platform. Reservation & Session Service will depend on it for Station validation (Phase 2/3), and later we will emit station-related events (Phase 4).

## Objective
Create a clean .NET 8 Station Service with layered architecture, Swagger enabled, and health endpoints. No DB logic yet.

## Deliverables
- `StationService.sln`
- `StationService.Api`, `StationService.Application`, `StationService.Domain`, `StationService.Infrastructure`
- Swagger UI enabled (Development)
- Health endpoints:
  - `/health/live`

## Constraints
- Controllers + DTOs (no Minimal APIs)
- Keep structure compatible with later Postgres + Helm + K8s
- No messaging in this task

## Acceptance criteria
- `dotnet build` passes
- `dotnet run` starts service
- Swagger loads and shows endpoints
- `/health/live` returns 200

## Stop condition
Stop after build/run + Swagger and health endpoint validation.
