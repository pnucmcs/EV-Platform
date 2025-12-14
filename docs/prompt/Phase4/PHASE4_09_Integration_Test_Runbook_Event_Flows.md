# Phase 4 – Task 09: Integration Runbook for Event Flows

## Task Type
Documentation / QA

## Context
As services expand, you need a repeatable way to validate event flows.
This is critical in your “distributed infra” environment to reduce confusion.

## Objective
Create an integration runbook that validates:
- HTTP flows
- Outbox persistence
- RabbitMQ publish
- Consumer receive

## Deliverables
- `docs/phase4-integration-runbook.md` containing:
  1. Pre-checks (pods, Postgres reachability, RabbitMQ reachable)
  2. Topology check commands
  3. Trigger actions via curl/Swagger
  4. Verify outbox rows
  5. Verify RabbitMQ queues
  6. Verify consumer logs
  7. Failure modes and troubleshooting

## Constraints
- Commands must be copy/paste ready.
- Must include namespace names, service names, and ports used in Phase 3.

## Acceptance Criteria
- Someone can follow the runbook from scratch and validate the pipeline.
- Runbook includes the “RabbitMQ down/up” reliability test.

## Stop Condition
Stop after runbook is complete and validated once.
