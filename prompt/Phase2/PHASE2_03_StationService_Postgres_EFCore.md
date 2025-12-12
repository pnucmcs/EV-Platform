# Phase 2 – Task 03: Station Service PostgreSQL Integration

## Task Type
Feature

## Context
Station data must persist in PostgreSQL hosted on TrueNAS.

## Objective
Integrate EF Core with PostgreSQL and enable migrations.

## Deliverables
- DbContext
- EF Core mappings
- Migrations applied to Postgres
- /health/ready endpoint

## Constraints
- Connection via env vars
- No Docker-only shortcuts

## Acceptance Criteria
- Tables exist in Postgres
- CRUD works via Swagger
- Health check validates DB

## Stop Condition
Stop after persistence is verified.
