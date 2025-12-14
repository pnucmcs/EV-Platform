# Phase 6 – Task 05: Telemetry Simulator (Generator Service)

## Task Type
Feature / Supporting Service

## Context
To make the platform demonstrable, we need generated telemetry.
A simulator creates believable readings and drives downstream behaviors.

## Objective
Create a Telemetry Simulator that:
- pulls station/charger/device IDs (initially from StationService)
- emits telemetry to TelemetryService ingestion API at configurable intervals

## Deliverables
- New repo/service: TelemetrySimulator (simple .NET Worker or Python)
- Configuration:
  - StationService base URL
  - TelemetryService ingestion URL
  - interval, devices per station, noise profile
- Kubernetes deployment (CronJob or Deployment)

## Constraints
- Keep it deterministic/repeatable (seeded randomness).
- Must not overload your cluster.

## Acceptance Criteria
- Simulator runs in K8s and continuously posts readings.
- TelemetryService shows “latest” data updating.

## Stop Condition
Stop after simulator is running and telemetry data is visible.
