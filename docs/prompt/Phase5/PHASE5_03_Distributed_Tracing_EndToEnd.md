# Phase 5 – Task 03: Distributed Tracing (End-to-End)

## Task Type
Feature / Observability

## Context
Tracing is required to debug request flows across:
- Ingress
- StationService
- ReservationService
- RabbitMQ consumers

## Objective
Enable distributed tracing using OpenTelemetry.

## Deliverables
- Trace exporter configuration (Jaeger/Tempo/OTLP)
- Correlation between HTTP and messaging spans
- Documentation of trace flow

## Constraints
- Traces must propagate correlationId.
- Sampling configurable.

## Acceptance Criteria
- One request shows a full trace across services.
- Messaging spans appear linked.

## Stop Condition
Stop after traces are visible end-to-end.
