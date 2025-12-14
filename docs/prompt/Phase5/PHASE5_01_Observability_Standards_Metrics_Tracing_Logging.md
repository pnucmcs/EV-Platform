# Phase 5 – Task 01: Observability Standards (Metrics, Tracing, Logging)

## Task Type
Architecture / Hardening

## Context
By Phase 4, the platform is event-driven and distributed. Without observability,
debugging becomes guesswork. This phase formalizes **how** we observe the system.

Your infra already includes:
- Prometheus (TrueNAS)
- Grafana (TrueNAS)

We now standardize application-side instrumentation.

## Objective
Define and implement observability standards across all .NET microservices.

## Deliverables
1. Observability standards document:
   - Metrics naming conventions
   - Trace/span naming conventions
   - Required labels (service, environment, correlationId)
2. .NET OpenTelemetry baseline:
   - HTTP server instrumentation
   - HTTP client instrumentation
   - EF Core instrumentation
3. Structured logging baseline:
   - JSON logs
   - CorrelationId included
4. Configuration options:
   - Enable/disable tracing
   - Sampling rate

## Constraints
- Do not introduce vendor lock-in.
- Use OpenTelemetry SDKs.
- No custom metrics explosion.

## Acceptance Criteria
- All services emit metrics and traces consistently.
- Logs include correlationId and service name.

## Stop Condition
Stop after standards are documented and baseline instrumentation compiles.
