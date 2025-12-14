# Task — API hardening baseline (Phase 2)

## Task title
Add ProblemDetails error handling + Correlation IDs + Health endpoints baseline

## Task type
Hardening

## Context
We need operationally sound APIs before adding more services and Kubernetes deployment. This sets the platform-wide conventions.

## Objective
Implement consistent error handling with ProblemDetails, correlation ID propagation, and health endpoints in both Station and Reservation services.

## Deliverables
- Global exception middleware -> ProblemDetails
- Validation error responses -> ProblemDetails
- Correlation ID middleware:
  - Use incoming `X-Correlation-ID` or generate a GUID
  - Add it to response headers
  - Include in logs
- Health endpoints:
  - `/health/live`
  - `/health/ready` (DB check where DB is configured)

## Acceptance criteria
- Errors return structured ProblemDetails with correlationId included
- Logs include correlationId
- Health endpoints behave correctly

## Stop condition
Stop after endpoints and middleware are validated via Swagger and curl.
