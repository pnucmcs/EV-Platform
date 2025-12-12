# Task — Reservation Service Skeleton (Phase 2)

## Task title
Bootstrap Reservation & Session Service .NET 8 solution structure

## Task type
Feature

## Context
Reservation & Session Service owns reservation and charging session lifecycle. It will validate stations via HTTP in Phase 2, then move to events later.

## Objective
Create a clean .NET 8 Reservation Service with layered architecture, Swagger enabled, and health endpoints. No DB logic yet.

## Deliverables
- `ReservationService.sln`
- `ReservationService.Api`, `ReservationService.Application`, `ReservationService.Domain`, `ReservationService.Infrastructure`
- Swagger enabled (Development)
- `/health/live` endpoint

## Constraints
- Controllers + DTOs (no Minimal APIs)
- No messaging in this task

## Acceptance criteria
- `dotnet build` passes
- `dotnet run` starts service
- Swagger loads
- `/health/live` returns 200

## Stop condition
Stop after build/run + Swagger and health endpoint validation.
