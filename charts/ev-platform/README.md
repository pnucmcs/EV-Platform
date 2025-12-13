# ev-platform Umbrella Chart

Deploy StationService and ReservationService together.

## Prerequisites
- Helm installed
- Namespace created (`ev-platform-dev`) and ingress controller installed if you plan to enable ingress in subcharts.
- Images pushed to a registry reachable by the cluster.

## Install
```bash
helm install ev-platform ./charts/ev-platform -n ev-platform-dev \
  --set stationservice.image.repository=your-registry/station-service \
  --set stationservice.image.tag=latest \
  --set stationservice.secrets.postgresConnection="Host=192.168.1.191;Port=5432;Database=station_db;Username=admin;Password=admin" \
  --set reservationservice.image.repository=your-registry/reservation-service \
  --set reservationservice.image.tag=latest \
  --set reservationservice.secrets.postgresConnection="Host=192.168.1.191;Port=5432;Database=reservation_db;Username=admin;Password=admin" \
  --set reservationservice.env.services.stationServiceBaseUrl="http://stationservice-api.ev-platform-dev.svc.cluster.local"
```

## Upgrade
```bash
helm upgrade ev-platform ./charts/ev-platform -n ev-platform-dev
```

## Uninstall
```bash
helm uninstall ev-platform -n ev-platform-dev
```

## Notes
- Secrets are provided via values; avoid committing real secrets. Use `--set`, `--set-file`, or external secret management in CI/CD.
- Subchart ingress is disabled by default; enable at subchart level if needed.
- Namespace separation: change `-n ev-platform-dev` to target different environments.
