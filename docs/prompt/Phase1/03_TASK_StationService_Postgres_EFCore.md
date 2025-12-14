# Task — Station Service PostgreSQL + EF Core (Phase 2)

## Task title
Integrate PostgreSQL with Station Service using EF Core

## Task type
Feature

## Context
Station Service must persist stations/chargers and be deployable in Kubernetes. PostgreSQL is running externally on TrueNAS.

## Objective
Connect Station Service to PostgreSQL at `192.168.1.191:5432` using EF Core, migrations, and readiness checks.

## Deliverables
- DbContext + entity mappings
- EF Core migrations
- CRUD endpoints persist data in Postgres
- Health endpoints:
  - `/health/ready` checks DB connectivity

## Constraints
- Connection string via environment variables (and appsettings fallback for dev)
- No Docker Compose shortcuts that bypass TrueNAS

## Acceptance criteria
- `dotnet ef database update` succeeds
- Tables exist in Postgres
- Create/Get/List via Swagger works end-to-end
- `/health/ready` returns 200 when DB reachable and non-200 when DB unreachable

## Stop condition
Stop after DB persistence and readiness checks work.
