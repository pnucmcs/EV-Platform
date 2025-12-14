# Phase 2 – Task 08: Reservation → Station Validation (HTTP)

## Task Type
Spike / Feature

## Context
Reservation Service must validate Station existence via Station Service.

## Objective
Implement synchronous HTTP validation behind an interface.

## Deliverables
- IStationDirectoryClient
- Typed HttpClient
- Config-driven StationService URL

## Acceptance Criteria
- Invalid StationId rejected
- Valid StationId accepted

## Stop Condition
Stop after end-to-end validation works.
