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
