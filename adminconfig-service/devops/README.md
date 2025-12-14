# Admin Config Service DevOps (Helm)

Kubernetes assets for building and deploying the Admin Config Service with Helm.

## Layout
- `devops/Dockerfile` – multi-stage .NET 8 build/publish image.
- `devops/helm/adminconfigservice` – Helm chart (Deployment/Service/Secret/ServiceAccount).
- `devops/helm/adminconfigservice/values.yaml` – deployment values (dev defaults, sample DB conn).

## Build & push the container
```sh
docker build -f devops/Dockerfile -t ghcr.io/your-org/adminconfigservice:<tag> .
docker push ghcr.io/your-org/adminconfigservice:<tag>
```
Update the `image.repository`/`image.tag` in the values file to match what you push.

## Deploy with Helm
```sh
helm upgrade --install adminconfigservice devops/helm/adminconfigservice \
  -f devops/helm/adminconfigservice/values.yaml
```

## Configuration & secrets
- App config is passed via env vars from values (`ASPNETCORE_ENVIRONMENT`, `Kestrel__Endpoints__Http__Url`, OpenTelemetry settings).
- Database connection lives in `connectionStrings.postgres`; the chart renders it into a Secret and mounts it as `ConnectionStrings__Postgres`.
- Replace placeholder connection strings before deploying; for stronger security, source them from an external secret manager or supply a separate, untracked values file.

## Notes
- Service listens on port 8080 and exposes `/health/ready` and `/health/live` probes.
- Tune resources/replicas/telemetry by editing the Helm values file.
