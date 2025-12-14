# Phase 2 – Task 02: Station Service Domain Model

## Task Type
Feature

## Context
Defines the core business entities for EV stations and chargers.

## Objective
Implement domain entities, enums, and basic invariants for Station Service.

## Deliverables
- Station entity
- Charger entity
- Enums:
  - StationStatus
  - ChargerStatus
  - ConnectorType
- UTC timestamps
- Domain-level validation methods

## Acceptance Criteria
- Domain project builds
- Entities enforce invariants (non-null name, valid coordinates)

## Stop Condition
Stop after entities compile and are referenced by Application layer.
