# Gateway DevOps (Helm)

Kubernetes assets for building and deploying the gateway with Helm.

## Layout
- `devops/Dockerfile` – multi-stage .NET 8 build/publish image.
- `devops/helm/gateway` – Helm chart (Deployment/Service/Secret/ServiceAccount).
- `devops/helm/gateway/values.yaml` – deployment values (dev defaults, sample settings).

## Build & push the container
```sh
docker build -f devops/Dockerfile -t ghcr.io/your-org/gateway:<tag> .
docker push ghcr.io/your-org/gateway:<tag>
```
Update the `image.repository`/`image.tag` in the values file to match what you push.

## Deploy with Helm
```sh
helm upgrade --install gateway devops/helm/gateway   -f devops/helm/gateway/values.yaml
```

## Configuration & secrets
- App config is passed via env vars from values (ports, telemetry, integrations).
- Database connection lives in `connectionStrings.postgres`; the chart renders it into a Secret and exposes it as `ConnectionStrings__Postgres`.
- Replace placeholder connection strings or override them with a separate values file before deploying.

## Notes
- Service listens on port 8080 and exposes `/health/ready` and `/health/live` probes when enabled in values.
- Tune resources/replicas/telemetry by editing the Helm values file.
