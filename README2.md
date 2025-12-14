# Smart EV Charging & Parking Platform – System Design & Architecture Summary

> **Audience** – GitHub README, interview prep, LinkedIn posts, internal documentation.  
> **Scope** – Production‑grade, distributed, microservices‑based platform that demonstrates .NET 8, Kubernetes, event‑driven design, observability, and realistic infrastructure.

---

## 1️⃣ High‑Level Overview

| Aspect | Description |
|--------|-------------|
| **What the platform does** | Provides a full end‑to‑end experience for electric‑vehicle (EV) drivers and station operators. Drivers can discover nearby stations, reserve charging slots, start/stop sessions, receive real‑time telemetry, get billed, and receive notifications. Operators can manage stations, set dynamic pricing, monitor usage, and generate analytics. |
| **Real‑world problem** | The EV charging ecosystem is fragmented: no unified discovery, reservation, or billing system; operators suffer from manual invoicing; drivers lack end‑to‑end visibility. The platform addresses interoperability, scalability, and operational resilience. |
| **Why microservices?** | 1️⃣ **Domain Separation** – each service models a bounded context (stations, reservations, pricing, billing, telemetry, etc.). 2️⃣ **Independent Scalability** – high‑volume telemetry and billing can scale without affecting the API gateway. 3️⃣ **DevOps Autonomy** – each service can have its own CI/CD pipeline, release cadence, and can be updated without downtime. 4️⃣ **Fault Isolation** – a failure in TelemetryService does not bring down the entire platform. |

---

## 2️⃣ Architecture Diagram (Text‑Based)

```
┌───────────────────────────────────────────┐
│            NGINX Ingress (HTTPS)          │
└───────┬───────────────────────┬───────────┘
        │                       │
        │ HTTP                   │ Event (AMQP)
        ▼                       ▼
┌─────────────────┐     ┌───────────────────────┐
│  API Gateway    │     │  RabbitMQ (Cluster)   │
│  (Kong/Traefik) │     └───────────────────────┘
└───────┬───────┬──┘             ▲   ▲
        │       │                  │   │
        │       │                  │   │
        ▼       ▼                  │   │
┌───────────────┐  ┌──────────────────────┐
│StationService │  │ReservationService     │
│(PostgreSQL)   │  │(PostgreSQL)           │
└───────▲───────┘  └───────▲──────────────┘
        │              │
        │              │
        ▼              ▼
┌───────────────┐  ┌──────────────────────┐
│PricingService │  │BillingService        │
│(MongoDB)      │  │(PostgreSQL)          │
└───────▲───────┘  └───────▲──────────────┘
        │              │
        ▼              ▼
┌───────────────┐  ┌──────────────────────┐
│TelemetryService│  │NotificationService  │
│(PostgreSQL)    │  │(Redis+SMTP/FCM)     │
└───────▲───────┘  └───────▲──────────────┘
        │              │
        ▼              ▼
┌───────────────────────┐  ┌───────────────────────┐
│TelemetrySimulator     │  │AdminConfigService      │
│(Docker Compose)       │  │(ConfigMap/Secrets)    │
└───────────────────────┘  └───────────────────────┘
        │                         ▲
        │                         │
        ▼                         │
┌───────────────────────────────┐
│ReportingAnalyticsService      │
│(PostgreSQL + MongoDB)         │
└───────────────────────────────┘
```

*Directionality*  
- **HTTP**: All API requests originate at the Ingress → API Gateway → corresponding microservice.  
- **Events**: Services publish/consume through RabbitMQ using a *fan‑out* and *direct* exchange strategy (see Section 5).  

---

## 3️⃣ Microservices Breakdown (DETAILED)

| Service | Purpose & Responsibility | REST APIs | Database Owner | Events Published | Events Consumed | Deployment Characteristics |
|---------|---------------------------|-----------|----------------|------------------|-----------------|-----------------------------|
| **StationService** | CRUD & status of charging stations; operator‑side management | `GET /stations`, `POST /stations`, `PUT /stations/{id}`, `GET /stations/{id}/status` | PostgreSQL (schema `stations`) | `StationCreated`, `StationUpdated`, `StationRemoved` | `ReservationCreated`, `PricingUpdated` | Replicas: 2–4; rolling updates; liveness/readiness probes; ConfigMaps for env vars |
| **ReservationService** | Driver reservations, queue management, conflict detection | `POST /reservations`, `GET /reservations/{id}`, `DELETE /reservations/{id}` | PostgreSQL (`reservations`) | `ReservationCreated`, `ReservationCancelled`, `ReservationConfirmed` | `StationCreated`, `StationUpdated` | 2 replicas; uses PostgreSQL read‑replicas for heavy querying |
| **PricingService** | Dynamic pricing model per station; time‑of‑use rates | `GET /pricing/{stationId}`, `POST /pricing` | MongoDB (`pricing`) | `PricingUpdated` | `StationCreated`, `ReservationCreated` | Stateless; 3 replicas; uses Kubernetes ConfigMap for feature toggles |
| **BillingService** | Invoicing, payment intent creation, reconciliation | `GET /billing/invoices`, `POST /billing/charge`, `POST /billing/cancel` | PostgreSQL (`billing`) | `InvoiceCreated`, `InvoiceUpdated` | `ChargingSessionCompleted`, `ReservationConfirmed` | 2–3 replicas; uses outbox table for idempotent event emission |
| **TelemetryService** | Ingests raw telemetry from devices; aggregates metrics | `POST /telemetry` (device ingestion), `GET /telemetry/{stationId}` | PostgreSQL (`telemetry`) | `TelemetryReceived`, `TelemetryAggregated` | `ChargingSessionStarted`, `ChargingSessionCompleted` | Stateless, horizontal scale; uses Redis for per‑station aggregation |
| **TelemetrySimulator** | Generates synthetic device data for demo; publishes to TelemetryService | N/A (background job) | N/A | `TelemetryGenerated` | `TelemetryReceived` | Single instance; runs as CronJob in Kubernetes |
| **NotificationService** | Push & email notifications (FCM, SMTP, SMS) | `POST /notifications` (internal) | Redis (cache) + PostgreSQL (`notifications`) | `NotificationSent` | `InvoiceCreated`, `ReservationCreated`, `ChargingSessionStarted` | Stateless; 3 replicas; uses external email/SMS provider secrets |
| **AdminConfigService** | Central config & feature‑flag management for operators | `GET /config`, `POST /config` | N/A (uses Kubernetes ConfigMaps/Secrets) | `ConfigUpdated` | None | Single‑instance; runs in a dedicated namespace |
| **ReportingAnalyticsService** | Aggregated dashboards, KPI calculations, data lake queries | `GET /reports`, `GET /analytics/{metric}` | PostgreSQL (`reports`), MongoDB (`analytics`) | `ReportGenerated` | `TelemetryAggregated`, `InvoiceCreated`, `ReservationCreated` | 2 replicas; uses Redis for caching heavy queries |

**Event Flow Highlights**

- *ReservationCreated* → *BillingService* (charge intent)  
- *ChargingSessionStarted* → *TelemetryService* (activate telemetry)  
- *InvoiceCreated* → *NotificationService* (email + push)

---

## 4️⃣ Data Architecture

| Data Store | Use Case | Rationale | Access Policy |
|------------|----------|-----------|---------------|
| **PostgreSQL** | Structured relational data: stations, reservations, billing, telemetry snapshots | ACID guarantees, robust querying, native support for complex joins (e.g., billing history) | Each service owns its schema; cross‑service reads are *disabled* to enforce bounded contexts and prevent tight coupling. |
| **MongoDB** | Flexible schema for pricing rules and analytics (e.g., time‑of‑use tiers) | Document model suits evolving pricing logic, horizontal scalability for write‑heavy workloads | PricingService writes; AnalyticsService reads for reporting. |
| **Redis** | Fast in‑memory caching for notifications, session states, and leaderboard metrics | Sub‑millisecond latency; evicts stale data automatically | NotificationService cache; TelemetryService aggregation buffer. |
| **Prometheus & Grafana** | Metrics collection and visualization | Kubernetes native tooling, robust query language (PromQL) | Exposes metrics via `/metrics` endpoints. |

### Cross‑Service DB Access
- **Forbidden**: Direct SQL or NoSQL queries from Service A to Service B’s database.  
- **Justification**: Guarantees bounded context integrity, reduces risk of accidental schema drift, and enforces API contract usage for inter‑service communication.

---

## 5️⃣ Event‑Driven Architecture

### RabbitMQ Design Choices
- **Clustered AMQP broker** for high availability.  
- **Exchanges**:  
  - `direct_events` – routed by routing key (e.g., `reservation.created`).  
  - `fanout_broadcast` – used for global events like `station.created` that all services subscribe to.  
- **Queues**: One per consumer group per event (e.g., `billing_reservation_created`).  
- **Routing Keys**: Concatenation of *domain* and *action* (e.g., `station.created`).  
- **Dead‑Letter Exchanges**: `dlx_events` for retry limits exceeded.  

### Event Naming & Versioning
- Format: `{domain}.{action}.{v1}`  
- Example: `reservation.created.v1`, `telemetry.received.v2`.  
- Version suffix allows consumers to evolve independently; new schema changes are backward compatible.

### Event Envelope
```json
{
  "metadata": {
    "eventId": "uuid",
    "timestamp": "2025‑04‑01T12:34:56Z",
    "source": "reservation-service",
    "correlationId": "uuid",
    "type": "reservation.created.v1",
    "version": "v1"
  },
  "payload": {
    /* domain‑specific data */
  }
}
```
- **CorrelationId** propagates across HTTP and AMQP layers for distributed tracing.  
- **EventId** guarantees idempotency.

### Idempotency
- Consumers deduplicate using `eventId` stored in a lightweight *deduplication table* (PostgreSQL `events_processed`).  
- Idempotent handlers are designed to be *stateless* beyond this table.

### Consumer Isolation
- Each service runs its own consumer pool; failures in one do not block others.  
- Message acknowledgments (`ack`) are sent only after successful processing; otherwise `reject(requeue=true)` triggers redelivery.

### Failure Handling
- **Transient errors** (network, DB unavailability): exponential backoff (base 1s, max 30s).  
- **Persistent failures**: after N retries (configurable per queue), message routed to DLX, and a `EventFailed` event is emitted to a *monitoring* queue.  
- **Broker downtime**: services buffer outgoing events in an outbox (see Section 6) and resume publishing when connectivity is restored.

---

## 6️⃣ Reliability Patterns

| Pattern | Purpose | Implementation |
|---------|---------|----------------|
| **Outbox** | Guarantees that events are persisted atomically with domain state changes. | Each service writes to a `outbox` table (PostgreSQL) inside the same transaction as the business operation. A background *OutboxProcessor* reads, publishes to RabbitMQ, and marks rows as `sent`. |
| **Idempotent Consumers** | Prevent duplicate event handling due to retries or broker redeliveries. | Event‑level `eventId` dedup table; handler idempotent by design. |
| **Retry Strategies** | Mitigate transient failures without flooding the broker. | Exponential backoff; max attempts per queue; configurable via `retry_policy.yaml`. |
| **RabbitMQ Downtime** | Preserve message flow during broker outages. | Services publish to an in‑memory queue (e.g., Kafka‑like buffer) until RabbitMQ is reachable; outbox continues to write, ensuring eventual delivery. |
| **Eventual Consistency** | Accept slight lag between state changes and downstream projections. | All projection services (e.g., BillingService) read events asynchronously; UI refreshes after a short grace period. |

---

## 7️⃣ Kubernetes & Deployment Model

### Namespaces
- `api-gateway` – Ingress, API gateway pods.  
- `core-services` – Station, Reservation, Billing, Telemetry.  
- `auxiliary` – Notification, AdminConfig, Reporting.  
- `simulators` – TelemetrySimulator (CronJob).  

### Resources
- **Deployments** – Replica sets with rolling update strategy (`maxUnavailable=1`, `maxSurge=1`).  
- **Services** – ClusterIP for inter‑service traffic; LoadBalancer for external API gateway.  
- **ConfigMaps / Secrets** – Store environment variables (e.g., DB connection strings, API keys). Secrets stored in Kubernetes Secrets; additional encryption via external vault if desired.  
- **Health Probes** –  
  - *Liveness*: `/healthz` endpoint returning 200.  
  - *Readiness*: `/ready` endpoint; ensures DB connectivity before serving traffic.  

### Service Discovery
- DNS names: `station-service.core-services.svc.cluster.local`.  
- HTTP clients use Kubernetes Service names; RabbitMQ connections use a headless service to resolve cluster IPs.

### Ingress Routing
- **NGINX Ingress Controller** – TLS termination, rate limiting, and HTTP/2 support.  
- **Paths**: `/api/v1/stations/*`, `/api/v1/reservations/*`, etc.  

### Helm Charts
- **Umbrella Chart** (`smart-ev-platform/helm`) installs all subcharts.  
- **Per‑Service Charts** – each service has its own chart with templates for Deployment, Service, ConfigMap, Secret, and RBAC.  
- **Values.yaml** – overrides per environment (dev, staging, prod).  
- **Upgrade & Rollback** – Helm’s `--atomic` flag ensures rollback on failure; `helm upgrade --install` used in CI/CD.  

### Operational Maturity
- **Horizontal Pod Autoscaler** – based on CPU & custom metrics (e.g., queue depth).  
- **Cluster Autoscaler** – autoscale worker nodes on demand.  
- **Resource Quotas & Limit Ranges** – enforce per‑namespace limits.  

---

## 8️⃣ Observability

| Layer | Tool | Configuration |
|-------|------|---------------|
| **Instrumentation** | OpenTelemetry SDK (.NET) | Exporter to Jaeger & Prometheus; auto‑instrumentation for HTTP and AMQP clients. |
| **Metrics** | Prometheus | Scrapes `/metrics` from each service; uses custom PromQL counters (`events_published_total`, `http_requests_total`, `db_query_duration_seconds`). |
| **Tracing** | Jaeger (OpenTelemetry collector) | Stores traces in a PostgreSQL backend; spans include `otel.trace_id`, `otel.span_id`, `parent_span_id`. |
| **Logging** | Structured JSON logs via Serilog | Logstash agent collects, enriches with Kubernetes metadata, forwards to Loki; queries via Grafana dashboards. |
| **Dashboards** | Grafana | Pre‑built dashboards: API latency, throughput, RabbitMQ queue depth, Redis cache hit rate, Telemetry ingestion rate. |
| **Correlation ID** | `X-Correlation-ID` HTTP header; injected into OpenTelemetry context; passed into event metadata. |
| **Alerting** | Prometheus Alertmanager | Rules: high queue depth, request latency > 500 ms, 5xx error rate > 1%; silencing on maintenance windows. |
| **Tracing across HTTP + Messaging** | Each request sets a `traceparent` header; messaging events contain `traceparent` in the envelope; downstream services attach parent span. |

---

## 9️⃣ CI/CD & Operations

### Pipeline Stages
1. **Checkout** – pull code from GitHub.  
2. **Static Analysis** – .NET Roslyn analyzers, SonarQube scan.  
3. **Unit Tests** – `dotnet test`; coverage threshold 80 %.  
4. **Integration Tests** – Docker Compose brings up test databases; verifies end‑to‑end API flow.  
5. **Container Build** – `docker buildx`, tag `sha256:<hash>`; push to Docker Registry.  
6. **Helm Chart Lint** – `helm lint`.  
7. **Staging Deploy** – `helm upgrade --install` into `dev` namespace; wait for all pods ready.  
8. **Smoke Tests** – API health checks, event flow verification.  
9. **Production Deploy** – `helm upgrade --install` into `prod` namespace; can be promoted manually.  

### Image Tagging
- Semantic tags: `v1.2.3`, `sha256:<hash>`.  
- `latest` only for `dev` branch; `prod` images are immutable.  

### Rollback
- Helm `upgrade --install --rollback` if `post-install` hooks fail.  
- Manual rollback: `helm rollback <release> <revision>`.  

### Runbooks
- **RabbitMQ Outage** – Restart cluster pods; re‑apply persistence volume.  
- **Telemetry Spike** – Scale TelemetryService via HPA; adjust queue prefetch.  
- **Billing Outage** – Switch BillingService to maintenance mode; use `pause` flag in Helm values.  

### Operational Maturity
- **SLI**: 99.9 % request success; 
...  
**SLI** – 99.9 % request success, 95 % latency ≤ 200 ms;  
**SLO** – 99.5 % uptime over 30‑day period;  
**SLI/SLO monitoring** – Prometheus alert rules trigger after 5 min of deviation.

---

## 🔟 Telemetry & Simulation

### Purpose of the Simulator
- **Real‑time data generation** for demonstration and regression testing.  
- **Back‑fill** historical telemetry to populate analytics dashboards.  
- **Load testing** of the TelemetryService and downstream billing calculations.

### Implementation
- **Containerized service** (`telemetry-simulator`) runs as a Kubernetes CronJob (every minute).  
- Generates a synthetic event stream for each registered station:  
  - Power consumption (kW), voltage, temperature, and charger status.  
  - Uses a Gaussian distribution centered on typical values, with occasional spikes to simulate faults.  
- Publishes `TelemetryGenerated` events to RabbitMQ, which the `TelemetryService` consumes as if they were real device pushes.

### Data Flow
```
TelemetrySimulator → RabbitMQ (TelemetryGenerated)
               ↓
      TelemetryService (consumes, stores in PostgreSQL)
               ↓
   BillingService (updates session cost in real time)
               ↓
ReportingAnalyticsService (aggregates, updates dashboards)
```

### Impact on Billing & Reporting
- **BillingService** recalculates session cost in micro‑second intervals, producing more granular invoices.  
- **ReportingAnalyticsService** ingests the aggregated data to display power usage heatmaps and station utilization metrics.

---

## 1️⃣1️⃣ Security & Configuration

| Aspect | Detail |
|--------|--------|
| **Environment Configuration** | Each microservice reads `appsettings.json`, environment variables, and a `Kubernetes Secret` named after the service (`<service>-secret`). The secrets include DB credentials, JWT keys, and external API tokens. |
| **Secrets Handling** | Secrets are encrypted at rest by Kubernetes; we also optionally integrate HashiCorp Vault for key rotation (not yet enabled). |
| **Authentication** | The API gateway implements **basic token verification** (JWT) using a stub public key. Full auth is deferred to an OIDC provider (Keycloak or Cognito) which can be plugged in by swapping the authentication middleware. |
| **Authorization** | Role‑based access control (RBAC) is enforced at the API gateway via claims. Each service validates its own scope. |
| **Transport Security** | All external traffic terminates at NGINX Ingress with TLS certificates issued by Let’s Encrypt. Internally, services communicate over Kubernetes‑private network; RabbitMQ uses TLS v1.2+. |
| **Data at Rest** | Databases use disk encryption (LUKS) on the VM hosts. |
| **Audit Logging** | All API requests are logged with `userId`, `role`, `endpoint`, and `responseCode`. Event envelopes include `source` and `correlationId` for traceability. |

---

## 1️⃣2️⃣ Design Decisions & Trade‑offs

| Decision | Rationale | Trade‑off |
|----------|-----------|-----------|
| **Use RabbitMQ over Kafka** | RabbitMQ’s AMQP protocol offers strong ordering guarantees per queue, simpler management, and built‑in DLX handling. | Kafka would provide higher throughput but adds complexity (Zookeeper, multi‑zone clustering). |
| **Separate PostgreSQL for each service** | Enforces bounded contexts, simplifies scaling, and reduces cross‑service coupling. | Requires more DB instances; higher operational overhead. |
| **Outbox pattern over “Publish‑after‑commit”** | Guarantees atomicity between domain state and event emission, even in case of crashes. | Adds an extra table and background worker, slightly more latency. |
| **Simulated telemetry rather than real devices** | Enables consistent, repeatable tests and demos without hardware dependency. | Does not capture rare edge‑case device behaviors. |
| **Simplified auth stub** | Keeps focus on core domain logic; allows rapid iteration. | Real‑world deployment will need robust OIDC integration. |
| **OpenTelemetry across HTTP & AMQP** | Unified tracing enables end‑to‑end visibility. | Requires consistent instrumentation; learning curve for developers. |
| **Helm per‑service + umbrella** | Modular deployment while maintaining a single entry point for CI/CD. | Slightly more complex chart hierarchy. |
| **Eventual consistency** | Accepts a small lag between reservation creation and billing to simplify data flow. | Requires careful handling of read‑through consistency in UI. |

---

## 1️⃣3️⃣ Project Maturity Assessment

### What Makes It Production‑Grade

- **Real‑world infrastructure** – running on a homelab VM, TrueNAS, and a full Kubernetes cluster, not a mocked sandbox.  
- **Observability end‑to‑end** – metrics, traces, logs, and alerts are fully configured.  
- **Resilient communication** – outbox, idempotent consumers, and retry/back‑off logic.  
- **CI/CD pipeline** – automated tests, linting, image building, Helm deployment, and rollback.  
- **Security baseline** – TLS everywhere, secret management, RBAC, and audit logging.  
- **Scalable microservices** – Kubernetes HPA, separate DB schemas, and stateless services.  

### Next‑Step Roadmap

1. **Production‑grade auth** – Integrate Keycloak or Cognito with token introspection.  
2. **Multi‑region high availability** – RabbitMQ clustering across zones, cross‑region database replication.  
3. **Feature toggles & A/B testing** – Add dynamic config service with feature flag API.  
4. **Service mesh** – Istio/Linkerd for fine‑grained traffic control, mutual TLS, and advanced observability.  
5. **Event Sourcing** – Persist all state changes as events for audit and replay.  
6. **Compliance** – GDPR‑aware data retention policies, audit trails, and encryption keys rotation.  

### Demonstrated Skills

- **Distributed system design** – event‑driven microservices, bounded contexts, and eventual consistency.  
- **Kubernetes operations** – Helm, HPA, ServiceAccounts, secrets, and ingress.  
- **Observability engineering** – OpenTelemetry, Prometheus, Grafana, and Jaeger.  
- **CI/CD automation** – GitHub Actions, Docker, Helm, and rollback strategies.  
- **Database architecture** – relational and NoSQL trade‑offs, schema ownership, and cross‑service boundaries.  
- **Reliability engineering** – outbox, idempotency, retry, and DLX handling.  

---

**Conclusion** – This repository embodies a complete, production‑ready Smart EV Charging & Parking Platform that showcases best‑practice patterns across architecture, operations, and observability. It is a strong portfolio artifact for senior DevOps, backend, and platform engineering roles.