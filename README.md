# Smart EV Charging & Parking Platform

This repository contains a .NET 8 microservices platform for EV charging and parking, designed for Kubernetes with Docker, Helm, RabbitMQ, Redis, PostgreSQL, MongoDB, and OpenTelemetry-based observability. It follows a monorepo layout for easier shared tooling, consistent CI/CD, and local compose-based development.

## Current Phase

- **Phase 0 — High-Level Architecture & Repo Strategy**: architecture summary, naming conventions, and proposed repository layout are documented in `docs/architecture.md`.

## Quick Links

- Architecture & naming: `docs/architecture.md`

## Next

- Proceed to Phase 1 to set up the local dev environment (dotnet, docker, kubectl, helm) once the Phase 0 artifacts are reviewed.

## Build and run StationService container locally

```bash
# Build image
docker build -f src/services/station-service/Api/Ev.Station.Api/Dockerfile -t station-service:local .

# Run with Postgres connection and port 8080 exposed
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Postgres="Host=192.168.1.191;Port=5432;Database=station_db;Username=admin;Password=admin" \
  -e Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080" \
  station-service:local
```

Health endpoints:
- `http://localhost:8080/health/live`
- `http://localhost:8080/health/ready`

## Build and run ReservationService container locally

```bash
# Build image
docker build -f src/services/reservation-service/Api/Ev.Reservation.Api/Dockerfile -t reservation-service:local .

# Run with Postgres connection and StationService base URL
docker run --rm -p 8081:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Postgres="Host=192.168.1.191;Port=5432;Database=reservation_db;Username=admin;Password=admin" \
  -e Services__StationService__BaseUrl="http://host.docker.internal:8080" \
  -e Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080" \
  reservation-service:local
```

Health endpoints:
- `http://localhost:8081/health/live`
- `http://localhost:8081/health/ready`


# Smart EV Charging & Parking Platform  
*Full‑stack, micro‑service architecture for real‑time EV charging and parking management*  

> **Scope** – This document is a single, technical reference that can be used as:  
> • GitHub README (with license, install, contribution sections)  
> • Interview briefing for Senior DevOps / Backend / Platform engineers  
> • LinkedIn technical post (copy‑pasteable bullet‑points)  
> • Architectural documentation for the engineering team  

> **Tone** – Precise, data‑driven, no marketing fluff.  
> **Audience** – Engineers, architects, product managers, security reviewers.  

---

## 1. Project Overview  

| Element | Description |
|---------|-------------|
| **Goal** | Provide a platform that enables: 1) dynamic reservation and real‑time monitoring of EV charging stations and parking spaces; 2) secure billing and payment; 3) analytics and reporting for operators. |
| **Users** | EV drivers (mobile/Web), charging station operators (operator portal), city planners (data export), partners (API consumers). |
| **Deployment** | Public cloud (AWS, GCP, or Azure) – multi‑region, Kubernetes‑managed. |
| **Compliance** | OCPP 1.6/2.0, ISO 15118, GDPR, PCI‑DSS (payment), ISO 27001 (security). |

---

## 2. Architectural Overview  

```
┌───────────────────────────────┐
│   Client Apps (React / Flutter)│
└─────────────┬──────────────────┘
              │
              ▼
┌──────────────────────────────────────────────────┐
│          API Gateway (Kong + Rate‑Limit / JWT)     │
└───────────────────────┬──────────────────────────┘
                          │
            ┌─────────────┴─────────────┐
            │                           │
            ▼                           ▼
┌─────────────────────┐      ┌─────────────────────┐
│   Auth & Identity   │      │  Notification Service│
│   (OAuth2.0 / OIDC) │      │   (Pub/Sub + Email/  │
└─────────────────────┘      │   Push)             │
                            └───────┬──────────────┘
                                    │
                                    ▼
          ┌───────────────────────────────────────┐
          │  Event Bus (Kafka – 1 topic per domain) │
          └─────────────────────┬─────────────────────┘
                                │
          ┌───────────────┬─────┴───────┬─────────────────┐
          │               │             │                 │
          ▼               ▼             ▼                 ▼
┌───────────────────┐ ┌───────────────────┐ ┌────────────────────┐
│   Charging Service│ │   Parking Service │ │  Billing Service   │
│   (Go)            │ │   (Go)            │ │  (Java / Spring)   │
└───────────────────┘ └───────────────────┘ └────────────────────┘
          │               │             │
          │               │             │
          ▼               ▼             ▼
┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────┐
│ PostgreSQL (relational│ │  TimescaleDB (ts‑data) │ │   Redis (cache)   │
│   for users, stations,│ │  for charging logs   │ │   for auth tokens │
│   bookings)           │ │   and analytics      │ │   and rate limits │
└─────────────────────┘ └─────────────────────┘ └─────────────────────┘
          │               │             │
          └──────┬────────┴───────────┬──────┘
                 │                     │
                 ▼                     ▼
       ┌─────────────────────┐ ┌─────────────────────┐
       │ Elasticsearch (search│ │  Grafana + Loki +   │
       │   for station search │ │  Prometheus)       │
       └─────────────────────┘ └─────────────────────┘
```

**Key architectural decisions**

1. **Micro‑service per bounded context** – each service owns its domain data.  
2. **Event‑driven communication** – Kafka ensures loose coupling and eventual consistency.  
3. **Polyglot persistence** – PostgreSQL for ACID transactions, TimescaleDB for high‑volume time series, Redis for fast reads, Elasticsearch for full‑text search.  
4. **API Gateway** – central entry point for authentication, routing, rate‑limiting, request/response transformations.  
5. **CI/CD** – GitHub Actions → Docker images → Argo CD for GitOps.  
6. **Observability** – OpenTelemetry instrumentation across services, Jaeger tracing, Loki logs.  

---

## 3. Technology Stack  

| Layer | Technology | Reasoning |
|-------|------------|-----------|
| **Client** | React 18, TypeScript; mobile – Flutter 3 | Rich UI, cross‑platform |
| **Auth** | Keycloak 21 (OIDC + OAuth2) | Open source, fine‑grained roles |
| **API Gateway** | Kong 3 (plugin architecture) | Handles JWT validation, rate limiting, dynamic routing |
| **Messaging** | Apache Kafka 3.5 (Confluent) | Distributed log, exactly‑once semantics |
| **Services** | Go 1.22 (high‑performance micro‑services) + Java 21 + Spring Boot (payment) | Go for I/O‑heavy services, Java for mature payment libs |
| **Databases** | PostgreSQL 15, TimescaleDB 3, Redis 7, Elasticsearch 8 | Complementary data models |
| **Infrastructure** | Kubernetes 1.28 (EKS / GKE / AKS), Helm 3 | Container orchestration |
| **CI/CD** | GitHub Actions, Docker, Argo CD | GitOps, declarative deployments |
| **Observability** | OpenTelemetry 1.22, Jaeger, Prometheus 2.56, Loki 2.9, Grafana 10 | Distributed tracing, metrics, logs |
| **Testing** | Go testing + Testify, JUnit + Testcontainers, Cypress, Newman | Unit, integration, contract, end‑to‑end |
| **Security** | TLS 1.3 everywhere, HashiCorp Vault 1.13 for secrets, OSSEC + Falco | Encrypted transport, secret vault, runtime security |

---

## 4. Domain Model  

### 4.1 Entities & Aggregates  

| Context | Aggregate | Key Attributes | Relationships |
|---------|-----------|----------------|---------------|
| **User** | `User` | `id`, `email`, `phone`, `roles`, `preferences` | owns `Booking` |
| **Station** | `ChargingStation` | `id`, `location`, `capacity`, `status`, `metadata` | contains `ChargingPoint` |
| **ChargingPoint** | `Point` | `id`, `connectorType`, `maxPower`, `status` | part of `ChargingStation` |
| **ParkingSpot** | `ParkingSpace` | `id`, `location`, `status`, `size` | part of `ParkingArea` |
| **Reservation** | `Booking` | `id`, `userId`, `stationId`, `startTime`, `endTime`, `status`, `paymentId` | links `User` → `ChargingStation` |
| **Session** | `ChargingSession` | `id`, `bookingId`, `meterStart`, `meterEnd`, `status`, `price` | extends `Booking` |
| **Payment** | `Payment` | `id`, `bookingId`, `amount`, `status`, `gatewayRef` | link to `Booking` |

### 4.2 Value Objects  

- `GeoPoint` (lat/lon, accuracy)  
- `Address` (street, city, postal, country)  
- `Price` (amount, currency)  

### 4.3 Domain Events  

| Event | Publisher | Consumers |
|-------|-----------|-----------|
| `StationAdded` | Station Service | Search, Notification |
| `ChargingStarted` | Charging Service | Billing, Notification |
| `ChargingCompleted` | Charging Service | Billing, Analytics |
| `ReservationCreated` | Parking Service | Billing, Notification |
| `PaymentSucceeded` | Billing Service | Notification, Analytics |

---

## 5. Micro‑service Responsibilities  

| Service | Core Responsibility | Primary API |
|---------|---------------------|-------------|
| **Auth** | User registration, login, role management | `/auth/*` |
| **Charging** | Station catalog, real‑time status, OCPP integration, session lifecycle | `/charging/*` |
| **Parking** | Spot catalog, reservation, availability, integration with city parking APIs | `/parking/*` |
| **Billing** | Transaction processing, invoicing, tax calculations, reporting | `/billing/*` |
| **Notification** | Email, SMS, push notifications, webhooks | `/notify/*` |
| **Analytics** | Aggregated metrics, time‑series data ingestion, dashboards | `/analytics/*` |
| **Search** | Elasticsearch indexing and query for stations/spots | `/search/*` |

> **Note** – All services expose a RESTful API (JSON) and publish/subscribe to Kafka topics.  
> **OpenAPI/Swagger** definitions are auto‑generated from code annotations.

---

## 6. Data Flow Examples  

### 6.1 Charging Session Lifecycle  

1. **Client** requests nearest station → `/search/stations?location=...`  
2. **Search Service** queries Elasticsearch → returns station list.  
3. **Client** selects station → `/charging/booking` (POST) – *Reservation* event published.  
4. **Parking Service** checks spot availability → publishes `ReservationCreated`.  
5. **Charging Service** reserves a point, updates status to `RESERVED`.  
6. **Client** initiates session via OCPP `StartTransaction` – service updates to `ACTIVE`.  
7. **Charging Service** streams real‑time power data to client via WebSocket.  
8. **On stop** – OCPP `StopTransaction` triggers `ChargingCompleted` event.  
9. **Billing Service** consumes event, calculates fee, calls payment gateway, updates `Payment` status.  
10. **Notification Service** sends confirmation email + receipts.

### 6.2 Parking Reservation  

1. **Client** calls `/parking/availability?location=...` → `Parking Service` queries DB.  
2. **Client** books spot → `/parking/reserve` → `Booking` created.  
3. **Parking Service** publishes `ReservationCreated`.  
4. **Billing Service** pre‑authorizes payment.  
5. **On expiry** or cancel → `ReservationCancelled` event triggers refund if needed.

---

## 7. Infrastructure & Deployment  

| Component | Tool | Reasoning |
|-----------|------|-----------|
| **Cluster** | Amazon EKS (or GKE/Azure AKS) | Managed Kubernetes, auto‑scaling |
| **Deployment** | Helm 3 (charts per service) + Argo CD | Declarative GitOps |
| **Secrets** | HashiCorp Vault + Kubernetes Secrets (encryption) | Centralized secret management |
| **Service Mesh** | Istio 1.20 (optional) | Traffic mirroring, mTLS |
| **Ingress** | NGINX Ingress Controller | HTTP/HTTPS routing to API Gateway |
| **CI/CD** | GitHub Actions → Docker Hub / GHCR | Automated build, test, scan, push |
| **Runtime** | Docker 20+ (multi‑stage builds) | Image isolation |
| **Observability** | OpenTelemetry Collector (sidecar) | Unified instrumentation |
| **Logging** | Loki + Grafana | Aggregated logs with query UI |
| **Monitoring** | Prometheus + Grafana | Service metrics, SLO dashboards |
| **Tracing** | Jaeger (operator) | Distributed request traces |
| **Alerting** | Alertmanager | PagerDuty integration |
| **Backup** | Velero for Kubernetes resources + pgBackRest for Postgres | Disaster recovery |
| **CDN** | CloudFront / Cloudflare | Static assets (React, images) |

### 7.1 Deployment Pipeline (GitHub Actions)  

```yaml
name: CI/CD

on:
  push:
    branches: [ main, release/* ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        service: [ auth, charging, parking, billing, notification, search, analytics ]
    steps:
      - uses: actions/checkout@v4
      - name: Set up Go
        if: contains(matrix.service, 'go')
        uses: actions/setup-go@v5
        with: { go-version: '1.22' }
      - name: Set up Java
        if: contains(matrix.service, 'java')
        uses: actions/setup-java@v4
        with: { distribution: 'temurin', java-version: '21' }
      - name: Build Docker image
        run: |
          docker build -t ghcr.io/$GITHUB_REPOSITORY/${{ matrix.service }}:$GITHUB_SHA .
      - name: Run tests
        run: |
          if [[ ${{ matrix.service }} == *'go'* ]]; then go test ./...; fi
          if [[ ${{ matrix.service }} == *'java'* ]]; then mvn test; fi
      - name: Publish to GHCR
        run: |
          echo ${{ secrets.GHCR_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
          docker push ghcr.io/$GITHUB_REPOSITORY/${{ matrix.service }}:$GITHUB_SHA
  deploy:
    needs: build
    runs-on: ubuntu-latest
    environment:
      name: production
    steps:
      - uses: actions/checkout@v4
      - name: Deploy to Argo CD
        run: |
          kubectl config use-context prod
          argocd app sync ${{ github.repository }}
```

---

## 8. Observability & Monitoring  

| Layer | Metric | Tool | SLO Example |
|-------|--------|------|-------------|
| **Service** | Latency (p95) | Prometheus + Grafana | `< 200 ms` for `/charging/start` |
| **Database** | Query latency | Prometheus node_exporter | `< 5 ms` average |
| **Kafka** | Partition lag | Kafdrop + Grafana | `lag < 10` |
| **Infrastructure** | CPU / Memory | Prometheus | CPU < 80% |
| **Security** | Audit log events | Loki | All auth events recorded |

**Tracing** – Every incoming request receives a `trace-id` via OpenTelemetry. Traces captured in Jaeger, correlated with logs (via `trace-id` field).  

**Alerts** – Thresholds for `ChargingService` failures, `BillingService` payment errors, `AuthService` login attempts (> 5 per minute) trigger PagerDuty escalations.

---

## 9. Security Architecture  

| Layer | Controls | Tools |
|-------|----------|-------|
| **Transport** | TLS 1.3 everywhere, mTLS for inter‑service | Istio