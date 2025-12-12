# Phase 6 – Task 06: Event Consumption – Billing on Session Completion

## Task Type
Feature

## Context
We now connect the event-driven workflow:
ReservationService publishes `session.completed.v1` → BillingService consumes → creates invoice.

## Objective
Implement BillingService consumer for `session.completed.v1` with idempotent invoice creation.

## Deliverables
- RabbitMQ consumer (using Phase 4 messaging library/standards)
- Handler for `ChargingSessionCompletedV1`
- Idempotency:
  - unique constraint on sessionId in invoices
  - dedupe store for eventId (optional)
- Optional call to PricingService:
  - compute final cost using tariff rules (HTTP call behind interface)

## Constraints
- If PricingService is unavailable, invoice can be created as Pending with amount=0 and retried later (document behavior).
- Consumer must not crash on malformed messages.

## Acceptance Criteria
- Completing a session triggers invoice creation automatically.
- Invoices can be retrieved via Billing APIs.
- System is stable under repeated event delivery.

## Stop Condition
Stop after end-to-end event flow works.
