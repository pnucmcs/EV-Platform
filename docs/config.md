# Configuration Standardization (StationService & ReservationService)

Both services use the same configuration keys and can be driven entirely by environment variables. Defaults target local/dev; production should always provide explicit values.

## Keys (shared)
- `ConnectionStrings:Postgres` — PostgreSQL connection string
- `Kestrel:Endpoints:Http:Url` — HTTP bind URL (default `http://0.0.0.0:8080`)
- `App:Name` / `App:Version` / `App:Environment` — service identity metadata

## ReservationService-specific
- `Services:StationService:BaseUrl` — StationService base URL for HTTP validation

## Environment variable equivalents
- `ConnectionStrings__Postgres`
- `Kestrel__Endpoints__Http__Url`
- `App__Name`, `App__Version`, `App__Environment`
- `Services__StationService__BaseUrl` (ReservationService)

## Examples

### Local `dotnet run` (StationService)
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__Postgres="Host=192.168.1.191;Port=5432;Database=station_db;Username=admin;Password=admin"
export Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080"
dotnet run --project src/services/station-service/Api/Ev.Station.Api/Ev.Station.Api.csproj
```

### Local `dotnet run` (ReservationService)
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__Postgres="Host=192.168.1.191;Port=5432;Database=reservation_db;Username=admin;Password=admin"
export Services__StationService__BaseUrl="http://localhost:8080"
export Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080"
dotnet run --project src/services/reservation-service/Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj
```

### Docker run (StationService)
```bash
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__Postgres="Host=db:5432;Database=station_db;Username=app;Password=secret" \
  -e Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080" \
  station-service:latest
```

### Docker run (ReservationService)
```bash
docker run --rm -p 8081:8080 \
  -e ConnectionStrings__Postgres="Host=db:5432;Database=reservation_db;Username=app;Password=secret" \
  -e Services__StationService__BaseUrl="http://station-service:8080" \
  -e Kestrel__Endpoints__Http__Url="http://0.0.0.0:8080" \
  reservation-service:latest
```

### Kubernetes env injection (Deployment snippet)
```yaml
env:
  - name: ConnectionStrings__Postgres
    valueFrom: { secretKeyRef: { name: station-db, key: postgres } }
  - name: Services__StationService__BaseUrl
    value: "http://station-service:8080"
  - name: Kestrel__Endpoints__Http__Url
    value: "http://0.0.0.0:8080"
```

## Health endpoints
- `/health/live` — always 200 (liveness)
- `/health/ready` — 200 when dependencies reachable (DB), non-200 otherwise
