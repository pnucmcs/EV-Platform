# Phase 3 – Task 01: Configuration Standardization (Environment Variables + appsettings)

## Task Type
Hardening / Platform Baseline

## Context
Phase 3 is about making **StationService** and **ReservationService** deployable and repeatable on Kubernetes.  
Your infrastructure is distributed and must be treated as “real external dependencies”:

- PostgreSQL: `192.168.1.191:5432` (TrueNAS)
- MongoDB: `192.168.1.191:27017` (TrueNAS)
- Redis: `192.168.1.191:30059` (TrueNAS)
- Prometheus: `192.168.1.191:30104` (TrueNAS)
- Grafana: `192.168.1.191:30037` (TrueNAS)
- RabbitMQ: **inside Kubernetes VM**

Before containers and K8s YAML/Helm, both services must support:
- **Environment variable configuration** (12-factor)
- **Explicit port binding**
- **Service-to-service base URL configuration**
- **Health check endpoints** suitable for probes

## Objective
Standardize configuration for **both services** so the same build can run:
- locally with `dotnet run`
- in Docker
- in Kubernetes  
with configuration injected through environment variables (and optional appsettings fallback for dev).

## Deliverables
For **StationService** and **ReservationService**:

1. `appsettings.json` and `appsettings.Development.json` aligned to a single config model
2. Strongly-typed Options classes (at minimum):
   - `DatabaseOptions` (connection string, provider)
   - `ServiceEndpointsOptions` (StationService base URL for ReservationService)
   - `AppOptions` (service name, version, environment)
3. Consistent configuration keys (MUST use the same keys across services):
   - `ConnectionStrings:Postgres`
   - `Services:StationService:BaseUrl` (ReservationService only)
   - `Kestrel:Endpoints:Http:Url` (or equivalent bind setting)
4. Documented environment variables (in `docs/config.md`) showing examples for:
   - local `dotnet run`
   - docker `docker run`
   - kubernetes env var injection
5. Verify both services:
   - bind to port `8080` by default
   - expose `/health/live` and `/health/ready` (ready may be no-op if DB not wired yet)

## Constraints
- No “magic defaults” hidden in code. If a dependency exists, it must be configurable.
- No hard-coded IPs in code; IPs may appear only in:
  - `appsettings.Development.json` (optional)
  - docs/examples
  - Kubernetes manifests later
- Keep ready endpoint for DB checks (if already implemented in Phase 2).

## Acceptance Criteria
- Running either service with only env vars works:
  - `ASPNETCORE_ENVIRONMENT=Production`
  - `ConnectionStrings__Postgres=...`
  - `Kestrel__Endpoints__Http__Url=http://0.0.0.0:8080`
- Swagger loads (in Development) using the same env-based port.
- `/health/live` always returns 200.
- `/health/ready` returns 200 when configured dependencies are reachable (DB) and non-200 otherwise.

## Implementation Steps (Must Follow)
1. **Define config contract**
   - Create Options classes in `*.Api` or `*.Infrastructure` (preferred: `*.Api/Configuration`).
2. **Bind options in Program.cs**
   - `services.AddOptions<T>().BindConfiguration("Section")...`
3. **Normalize connection string handling**
   - Use `ConnectionStrings:Postgres` everywhere.
4. **Normalize Kestrel binding**
   - Set default bind to `http://0.0.0.0:8080`.
5. **Write docs**
   - `docs/config.md` with exact env var examples.

## Verification Steps
- StationService:
  - `dotnet run --project StationService.Api` with env vars set
  - Confirm it listens on `:8080` and health endpoints respond
- ReservationService:
  - same
  - Confirm it reads `Services:StationService:BaseUrl`

## Stop Condition
Stop once both services run with env-only configuration and docs are written.
