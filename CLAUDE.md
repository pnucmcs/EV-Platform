# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Core commands

| Purpose | Command | Notes |
|---------|---------|-------|
| Build all services | `dotnet build src/services/**/*.csproj` | Compiles every service using .NET 8 SDK.
| Lint / style check | `dotnet format --verify-no-changes src/services/**/*.csproj` | Checks code formatting and style, fails if changes would be made.
| Run all tests | `dotnet test src/services/**/*.csproj --no-build --logger "trx;LogFileName=test-results.trx"` | Executes unit tests for all services.
| Run single test | `dotnet test <PROJECT> --filter FullyQualifiedName=<namespace.ClassName.MethodName>` | Specify the fully‑qualified test name.
| Clean test caches | `dotnet clean src/services/**/*.csproj` | Removes compiled artifacts.

### Example

```bash
# Build everything
$ dotnet build src/services/**/*.csproj

# Lint
$ dotnet format --verify-no-changes src/services/**/*.csproj

# Run all tests
$ dotnet test src/services/**/*.csproj

# Run a single test:
$ dotnet test src/services/reservation-service/Tests/Ev.Reservation.Tests.csproj \
    --filter FullyQualifiedName=Ev.Reservation.Tests.ReservationIntegrationTests.CreateReservation
```

## Directory structure (high level)

- `src/services/` – contains each microservice's source.
  - Each service follows the pattern `Ev.<Domain>.Api`, `Ev.<Domain>.Application`, `Ev.<Domain>.Infrastructure`, `Ev.<Domain>.Domain`.
  - Project files live under `src/services/<service>/*`.
  - Tests are in `src/services/<service>/Tests/`.
- `deploy/` – Helm charts for deploying services to Kubernetes.
- `infra/` – Docker‑Compose files for local development (`docker‑compose.yml`).
- `docs/architecture.md` – describes the overall architecture and naming conventions.
- `.github/workflows/` – CI/CD pipeline definitions.

## Architectural overview

- The platform is a **service‑mesh**‑ready infrastructure using ASP.NET Core APIs, a BFF Gateway, RabbitMQ for asynchronous messaging, and PostgreSQL/MongoDB/Redis for persistence and caching.
- Each service exposes an HTTP API via `Ev.<Domain>.Api` and publishes/consumes events through a shared `ev.events` topic.
- OpenTelemetry is embedded in every service and reports metrics to Prometheus and logs to Loki.
- A shared Helm chart (`ev-platform`) provides an umbrella deployment; sub‑charts exist for each service.

## Local development

1. **Prerequisites** – .NET 8 SDK, Docker, Docker‑Compose, kubectl, Helm.
2. Start the local stack: `docker compose -f infra/docker-compose.yml up -d`
3. Build services as above.
4. Run tests.
5. Deploy a service: `helm upgrade --install ev-<domain>-svc deploy/helm/<domain> -n ev-platform-dev`.

Feel free to adapt commands to your workflow; these are the most common patterns used in the repo.