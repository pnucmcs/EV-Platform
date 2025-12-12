# Phase 2 – Task 01: Station Service Solution Scaffolding

## Task Type
Feature

## Context
Station Service is a core domain service that represents EV charging stations and chargers.
It is the authoritative source for station metadata and will be consumed by Reservation Service.

## Objective
Create a clean .NET 8 solution and layered project structure for Station Service.

## Deliverables
- StationService.sln
- Projects:
  - StationService.Api
  - StationService.Application
  - StationService.Domain
  - StationService.Infrastructure
- Project references wired correctly
- Swagger enabled
- /health/live endpoint

## Acceptance Criteria
- `dotnet build` succeeds
- `dotnet run` starts API
- Swagger UI loads

## Stop Condition
Stop after successful build and Swagger validation.
