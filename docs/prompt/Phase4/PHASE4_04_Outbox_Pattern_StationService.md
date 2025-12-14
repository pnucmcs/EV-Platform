# Phase 4 – Task 04: Outbox Pattern (StationService)

## Task Type
Hardening / Reliability

## Context
If StationService writes to Postgres and publishes an event, we must prevent the classic failure:
- DB write succeeded
- event publish failed
Result: inconsistent state for consumers.

The Outbox pattern provides eventual consistency and reliability.

## Objective
Implement an Outbox table and background dispatcher for StationService.

## Deliverables
In StationService:
1. Outbox table:
   - `outbox_messages`
     - id (GUID)
     - occurred_at_utc
     - type
     - payload_json
     - routing_key
     - correlation_id
     - processed_at_utc (nullable)
     - publish_attempts
     - last_error (nullable)
2. EF Core mapping + migration
3. On station create/status change:
   - write domain entity
   - write outbox message in same transaction
4. Background worker:
   - polls outbox table
   - publishes events to RabbitMQ
   - marks processed
   - retry with backoff and attempt limits
5. Configuration:
   - Outbox polling interval
   - Batch size

## Constraints
- Do NOT publish directly in the request handler when outbox is enabled.
- Publishing must be idempotent:
  - use eventId as messageId
- Keep polling simple and safe.

## Acceptance Criteria
- Creating a station results in an outbox message.
- Outbox dispatcher publishes event and marks message processed.
- If RabbitMQ is down, outbox rows remain unprocessed and retry later.

## Implementation Steps
1. Add outbox entity + DbSet to StationDbContext.
2. Add migration.
3. Modify command handlers to enqueue outbox event within transaction.
4. Add hosted service dispatcher.
5. Add logging and metrics counters (optional in Phase 4).

## Verification
- Stop RabbitMQ, create a station -> outbox row present, not processed.
- Start RabbitMQ -> dispatcher publishes and processes row.

## Stop Condition
Stop after outbox dispatch works reliably.
