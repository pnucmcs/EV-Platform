# Phase 5 – Task 02: OpenTelemetry Metrics Export to Prometheus

## Task Type
Feature / Observability

## Context
Prometheus is running externally. Services must expose `/metrics`
for scraping.

## Objective
Expose Prometheus-compatible metrics from all services.

## Deliverables
- OpenTelemetry metrics exporter configuration
- `/metrics` endpoint exposed
- Service labels applied

## Constraints
- Do not expose metrics publicly via Ingress.
- Metrics endpoint must be scrape-ready inside K8s.

## Acceptance Criteria
- Prometheus can scrape metrics from services.
- Metrics visible in Grafana.

## Stop Condition
Stop after metrics appear in Prometheus.
