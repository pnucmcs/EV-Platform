# Phase 4 – Task 02: RabbitMQ Topology (Exchange, Queues, Bindings) in Kubernetes

## Task Type
Infra / Platform Enablement

## Context
RabbitMQ is running inside your Kubernetes cluster VM. We need repeatable topology:
- Exchange(s)
- Consumer queues
- Bindings
so producers can publish consistently and consumers can subscribe reliably.

## Objective
Define and apply RabbitMQ topology for EV platform events.

## Deliverables
1. A topology definition approach (choose one and document):
   - Option A: RabbitMQ definitions.json loaded via Helm values (recommended)
   - Option B: Declarative topology via app startup (less ideal but OK initially)
2. Create exchange:
   - Name: `ev.platform`
   - Type: `topic`
   - Durable: true
3. Create queues (durable) for initial consumers:
   - `notification-service.events`
   - `billing-service.events` (even if Billing not implemented yet)
   - `telemetry-service.events` (placeholder)
4. Bindings with routing key patterns:
   - Notification: `station.*.v1`, `reservation.*.v1`, `session.*.v1`
   - Billing: `session.completed.v1` (and possibly reservation created)
5. Documentation:
   - `docs/rabbitmq-topology.md` with commands/screenshots optional

## Constraints
- Do not create one shared queue for all consumers.
- Consumers must have their own queue to avoid competing-consumer cross-domain coupling.
- Use durable exchange/queues.

## Acceptance Criteria
- Exchange exists and is durable.
- Queues exist and are durable.
- Bindings exist and match routing key patterns.
- Topology is reproducible (one command / one Helm upgrade).

## Implementation Steps
1. Identify current RabbitMQ Helm release and namespace.
2. Choose topology mechanism (definitions.json or automation).
3. Apply topology.
4. Validate with RabbitMQ management UI or CLI.

## Verification
- Provide evidence:
  - Exchange list
  - Queue list
  - Binding list

## Stop Condition
Stop after topology is created and verified in the running cluster.
