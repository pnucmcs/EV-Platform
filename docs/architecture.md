# Phase 0 — Architecture & Repo Strategy

## High-Level Architecture (text map)

- Clients (mobile/web) → NGINX Ingress → API gateway/BFF (ASP.NET Core) routes to internal services via HTTP; internal async events via RabbitMQ.
- **Auth & Identity Service** (HTTP + RabbitMQ) → PostgreSQL; issues JWTs used by all APIs via Ingress/Gateway.
- **User Profile Service** (HTTP) → PostgreSQL; caches profile basics in Redis.
- **Station & Parking Service** (HTTP + emits/handles events) → PostgreSQL (core data), MongoDB (station metadata/layout), Redis (availability cache).
- **Pricing & Tariff Service** (HTTP) → PostgreSQL; exposes tariff rules to Reservation.
- **Reservation & Session Service** (HTTP; publishes/consumes events) → PostgreSQL; uses Redis for availability locks; publishes `ReservationCreated`, `ChargingSessionStarted/Completed`.
- **Billing & Payment Service** (HTTP + events) → PostgreSQL; consumes reservation/session events; publishes `PaymentCompleted/Failed`.
- **Notification Service** (events) → consumes payment/reservation/session events; sends email/push (stub initially).
- **Telemetry & Device Service** (HTTP + events) → MongoDB for device configs; emits station status events.
- **Reporting & Analytics Service** (HTTP + events) → PostgreSQL warehouse/materialized views; consumes domain events.
- Shared observability stack: OpenTelemetry in services → Prometheus/Grafana/Loki/Jaeger; metrics scraped via Prometheus; logs shipped via Loki/Promtail.
- Service mesh (later): Istio for mTLS, traffic shaping, retries/timeouts.

## Broker Choice

- **RabbitMQ** for this MVP: simple ops footprint, strong routing (exchanges/queues/bindings), good fit for event-driven workflows and homelab-scale clusters. Kafka remains an optional upgrade when throughput/retention needs grow.

## Repository Strategy

- **Monorepo** for cohesive tooling, shared packages, unified CI/CD, and simpler local composition. Keeps service templates consistent and reduces boilerplate drift.
- Structure:
  - `src/`
    - `services/`
      - `gateway/` (BFF/API gateway)
      - `auth-service/`
      - `user-service/`
      - `station-service/`
      - `pricing-service/`
      - `reservation-service/`
      - `billing-service/`
      - `notification-service/`
      - `telemetry-service/`
      - `reporting-service/`
    - `shared/` (cross-cutting libs: messaging abstractions, OpenTelemetry setup, common DTO contracts)
  - `deploy/`
    - `helm/` (platform chart with subcharts per service; shared helpers)
    - `k8s/` (optional raw manifests for quick testing)
  - `infra/` (docker-compose for local stack: services + PostgreSQL + MongoDB + Redis + RabbitMQ + Prometheus/Grafana/Loki/Jaeger)
  - `docs/` (architecture, ADRs, runbooks)
  - `.github/workflows/` (CI/CD)

## Naming Conventions

- **Services & projects**: `ev-<domain>-service` (folder), C# projects `Ev.<Domain>.Api`, `Ev.<Domain>.Application`, `Ev.<Domain>.Infrastructure`, `Ev.<Domain>.Domain`.
- **Docker images**: `ghcr.io/<org>/ev-<domain>-service:<tag>` (e.g., `ghcr.io/your-org/ev-reservation-service:0.1.0`).
- **Kubernetes namespaces**: `ev-platform-dev`, `ev-platform-stg`, `ev-platform-prod`.
- **Helm release names**: `ev-<domain>-svc` (per service) or `ev-platform` (umbrella chart).
- **Ingress host**: `api.ev-platform.local` (dev), `api.ev-platform.example.com` (prod), paths like `/api/v1/stations`, `/api/v1/reservations`.
- **Environment variables**: `EV__<SERVICE>__<SETTING>` (aligns with ASP.NET Core double-underscore binding), e.g., `EV__POSTGRES__CONNECTIONSTRING`, `EV__RABBITMQ__HOST`.
- **Messaging**: exchanges `ev.events` (topic), routing keys `reservation.created`, `session.started`, `payment.completed`, `station.status.changed`.

## Initial MVP Scope (code first)

- Gateway/BFF, Auth & Identity, Station & Parking, Reservation & Session, Billing & Payment, Notification. Others stubbed later.
- Persistence: PostgreSQL (relational), MongoDB (station metadata), Redis (cache/locks), RabbitMQ (events).

## Rationale: Umbrella Helm Chart with Subcharts

- A platform chart (`ev-platform`) with subcharts per service balances reuse and modularity. Shared templates/values for telemetry, sidecars, and ingress live at the umbrella level; services remain individually deployable/overridable.

## Next Actions (Phase 1 preview)

- Validate tooling: dotnet 8 SDK, Docker, kubectl, Helm.
- Create solution scaffolding and initial service projects per `src/services/...`.
- Draft docker-compose for local stack.
