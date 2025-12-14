# Phase 6 – Task 02: PricingService (Scaffold + Domain + APIs)

## Task Type
Feature

## Context
PricingService is the authority for tariffs and price calculations.
Reservation and Billing will depend on pricing outputs.

## Objective
Implement a .NET 8 PricingService with:
- layered architecture
- PostgreSQL persistence
- tariff management APIs
- price estimation API

## Deliverables
1. Solution and projects:
   - PricingService.Api / Application / Domain / Infrastructure
2. Domain model (minimal but realistic):
   - TariffPlan (id, stationId optional, name, currency, baseRatePerKwh, idleFeePerMinute, validFromUtc, validToUtc, isActive)
   - TimeOfUseRule (optional): (dayOfWeek, startTime, endTime, multiplier)
3. APIs (versioned `/api/v1`):
   - CRUD TariffPlans
   - `POST /pricing/estimate`:
     - input: stationId, startTimeUtc, endTimeUtc, estimatedKwh
     - output: estimatedCost, breakdown
4. Validation + mapping + ProblemDetails
5. Health endpoints and Helm chart (reuse patterns from Phase 3)

## Constraints
- Keep calculation deterministic and explainable.
- Currency handling: store ISO currency code, avoid floating precision issues (use decimal).

## Acceptance Criteria
- Can create tariff plans and get estimates via Swagger.
- Data persists in Postgres schema owned by PricingService.
- Deployed via Helm into `ev-platform-dev`.

## Stop Condition
Stop after service is deployed and estimate endpoint works.
