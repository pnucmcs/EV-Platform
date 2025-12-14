# Billing Service DevOps (Helm)

Kubernetes assets for building and deploying the billing-service with Helm.

## Layout
- `devops/Dockerfile` – multi-stage .NET 8 build/publish image.
- `devops/helm/billingservice` – Helm chart (Deployment/Service/Secret/ServiceAccount).
- `devops/helm/billingservice/values.yaml` – deployment values (dev defaults, sample settings).

## Build & push the container
```sh
docker build -f devops/Dockerfile -t ghcr.io/your-org/billingservice:<tag> .
docker push ghcr.io/your-org/billingservice:<tag>
```
Update the `image.repository`/`image.tag` in the values file to match what you push.

## Deploy with Helm
```sh
helm upgrade --install billingservice devops/helm/billingservice   -f devops/helm/billingservice/values.yaml
```

## Configuration & secrets
- App config is passed via env vars from values (ports, telemetry, integrations).
- Database connection lives in `connectionStrings.postgres`; the chart renders it into a Secret and exposes it as `ConnectionStrings__Postgres`.
- Replace placeholder connection strings or override them with a separate values file before deploying.

## Notes
- Service listens on port 8080 and exposes `/health/ready` and `/health/live` probes when enabled in values.
- Tune resources/replicas/telemetry by editing the Helm values file.
