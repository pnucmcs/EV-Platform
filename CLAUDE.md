# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Core commands

| Purpose | Command | Notes |
|---------|---------|-------|
| Build all services | `dotnet build src/services/**/*.csproj` | Compiles every service using .NET 8 SDK. |
| Build a single service | `dotnet build src/services/<service>/*.csproj` | Replace `<service>` with the folder name (e.g. `reservation-service`). |
| Lint / style check | `dotnet format --verify-no-changes src/services/**/*.csproj` | Fails if formatting changes would be made. |
| Run all tests | `dotnet test src/services/**/*.csproj --no-build --logger "trx;LogFileName=test-results.trx"` | Executes unit tests for all services. |
| Run tests for a single project | `dotnet test src/services/<service>/Tests/<project>.csproj --no-build --logger "trx;LogFileName=test-results.trx"` | Replace `<service>` and `<project>` accordingly. |
| Run a single test | `dotnet test <PROJECT> --filter FullyQualifiedName=<namespace.ClassName.MethodName>` | Specify the fully‑qualified test name. |
| Clean test caches | `dotnet clean src/services/**/*.csproj` | Removes compiled artifacts. |

## Local development

1. **Prerequisites** – .NET 8 SDK, Docker, Docker‑Compose, kubectl, Helm.
2. Start the local stack: `docker compose -f infra/docker-compose.yml up -d`.
3. Build services: `dotnet build src/services/**/*.csproj`.
4. Run tests: `dotnet test src/services/**/*.csproj`.
5. Deploy a service: `helm upgrade --install ev-<domain>-svc deploy/helm/<domain> -n ev-platform-dev`.

## Architecture overview

- The platform is a service‑mesh‑ready infrastructure using ASP.NET Core APIs, a BFF gateway, RabbitMQ for asynchronous messaging, and PostgreSQL/MongoDB/Redis for persistence and caching.
- Each service exposes an HTTP API via `Ev.<Domain>.Api` and publishes/consumes events through a shared `ev.events` topic.
- OpenTelemetry is embedded in every service and reports metrics to Prometheus and logs to Loki.
- A shared Helm chart (`ev-platform`) provides an umbrella deployment; sub‑charts exist for each service.

## Directory structure (high level)

- `src/services/` – each microservice’s source (API, Application, Infrastructure, Domain projects).
- `deploy/` – Helm charts for deploying services to Kubernetes.
- `infra/` – Docker‑Compose files for local development (`docker‑compose.yml`).
- `docs/architecture.md` – detailed architecture and naming conventions.
- `.github/workflows/` – CI/CD pipeline definitions.

## Naming conventions

- **Services & projects**: `ev-<domain>-service` folder, C# projects `Ev.<Domain>.Api`, `Ev.<Domain>.Application`, `Ev.<Domain>.Infrastructure`, `Ev.<Domain>.Domain`.
- **Docker images**: `ghcr.io/<org>/ev-<domain>-service:<tag>`.
- **Kubernetes namespaces**: `ev-platform-dev`, `ev-platform-stg`, `ev-platform-prod`.
- **Helm release names**: `ev-<domain>-svc` or `ev-platform`.
- **Environment variables**: `EV__<SERVICE>__<SETTING>` (e.g., `EV__POSTGRES__CONNECTIONSTRING`).
- **Messaging**: exchanges `ev.events` (topic), routing keys `reservation.created`, `session.started`, `payment.completed`, `station.status.changed`.

## Observability

- OpenTelemetry is enabled in every service.
- Metrics are scraped by Prometheus.
- Logs are shipped to Loki via Promtail.
- Traces can be viewed in Jaeger.

## CI/CD

- GitHub Actions workflows in `.github/workflows/` build, lint, test, and deploy services.
- Pull requests trigger lint and test jobs.
- Releases deploy to the `ev-platform` Helm chart.
