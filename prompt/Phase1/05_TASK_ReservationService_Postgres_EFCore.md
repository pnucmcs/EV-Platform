# Task — Reservation Service PostgreSQL + EF Core (Phase 2)

## Task title
Integrate PostgreSQL with Reservation Service using EF Core

## Task type
Feature

## Context
Reservation Service must persist Reservations and ChargingSessions. Postgres is on TrueNAS. It must be Kubernetes-ready.

## Objective
Connect Reservation Service to PostgreSQL at `192.168.1.191:5432` with EF Core migrations, repositories, and readiness checks.

## Deliverables
- DbContext + entity mappings
- EF Core migrations
- CRUD endpoints for reservations and sessions persist in Postgres
- `/health/ready` checks DB connectivity

## Constraints
- Connection string via environment variables
- No cross-service DB access
- Keep overlap check logic minimal but correct

## Acceptance criteria
- Tables created and persisted
- Create reservation/session flows work via Swagger
- `/health/ready` returns 200 when DB reachable

## Stop condition
Stop after DB persistence and readiness checks work.
