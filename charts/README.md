# Helm Charts

Charts are provided per service to mirror the raw Kubernetes YAML. Install into `ev-platform-dev` (or your namespace of choice) after pushing images to a registry.

## StationService
```bash
helm install stationservice ./charts/stationservice -n ev-platform-dev \
  --set image.repository=your-registry/station-service \
  --set image.tag=latest \
  --set secrets.postgresConnection="Host=192.168.1.191;Port=5432;Database=station_db;Username=admin;Password=admin"

# Upgrade
helm upgrade stationservice ./charts/stationservice -n ev-platform-dev

# Remove
helm uninstall stationservice -n ev-platform-dev
```

## ReservationService
```bash
helm install reservationservice ./charts/reservationservice -n ev-platform-dev \
  --set image.repository=your-registry/reservation-service \
  --set image.tag=latest \
  --set secrets.postgresConnection="Host=192.168.1.191;Port=5432;Database=reservation_db;Username=admin;Password=admin" \
  --set env.services.stationServiceBaseUrl="http://stationservice-api.ev-platform-dev.svc.cluster.local"

helm upgrade reservationservice ./charts/reservationservice -n ev-platform-dev
helm uninstall reservationservice -n ev-platform-dev
```

## Notes
- Connection strings are set via values; avoid committing real secrets. Use `--set`, `--set-file`, or `--values` overrides in CI.
- Ingress templates are included but disabled by default (`ingress.enabled=false`); enable and align paths if you want chart-managed ingress.
- Default service ports: ClusterIP `:80` -> container `:8080`.
- Health probes are wired to `/health/live` and `/health/ready`.
