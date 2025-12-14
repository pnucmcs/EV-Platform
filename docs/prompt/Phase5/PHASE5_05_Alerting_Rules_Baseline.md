# Phase 5 – Task 05: Alerting Rules Baseline

## Task Type
Hardening / Reliability

## Context
Alerts are required to catch failures early.

## Objective
Define baseline alerting rules.

## Deliverables
- Prometheus alert rules for:
  - Service down
  - High error rate
  - Outbox backlog growing
  - RabbitMQ unreachable
- Alert severity classification

## Constraints
- Avoid alert noise.
- Alerts must be actionable.

## Acceptance Criteria
- Alerts trigger when simulated failures occur.

## Stop Condition
Stop after alerts validated.
