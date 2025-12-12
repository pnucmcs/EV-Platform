# Phase 6 – Task 07: Notification Workflows (Event-Driven)

## Task Type
Feature

## Context
NotificationService should evolve from “log-only consumer” into a workflow-driven consumer
that produces user-facing notifications (still without real email/SMS integrations).

## Objective
Implement notification workflows for key events:
- station.created.v1 (admin notification)
- reservation.created.v1 (user notification)
- session.started.v1
- session.completed.v1
- payment status changes (placeholder)

## Deliverables
- NotificationService handlers per event type
- Persistence (Postgres recommended):
  - Notifications table (id, userId, type, title, body, status, createdAtUtc)
- APIs (optional) to query notifications:
  - `GET /api/v1/users/{userId}/notifications`

## Constraints
- No real email/SMS; store notifications for now.
- Must be idempotent per eventId.

## Acceptance Criteria
- Triggering flows produces notification records.
- Records retrievable via API (if implemented) or logs clearly show creation.

## Stop Condition
Stop after notifications are stored for the key events.
