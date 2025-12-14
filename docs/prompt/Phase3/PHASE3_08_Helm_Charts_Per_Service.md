# Phase 3 – Task 08: Helm Charts (One Chart per Service)

## Task Type
Feature / Platform Enablement

## Context
Raw YAML is good for learning and validation, but Helm is required for repeatable deployments and environment-specific configuration.
Microservice ecosystems strongly benefit from “one chart per service” to decouple releases.

## Objective
Create Helm charts for StationService and ReservationService that replicate the working raw YAML deployments.

## Deliverables
- `charts/stationservice/` Helm chart:
  - templates: deployment, service, configmap, secret, hpa (optional), ingress (optional)
  - values.yaml with image, resources, env, probes
- `charts/reservationservice/` Helm chart:
  - same structure
- `values-dev.yaml` (optional) for dev overrides

Required values:
- image.repository, image.tag
- service.port (ClusterIP)
- env:
  - ASPNETCORE_ENVIRONMENT
  - Kestrel binding
  - Services__StationService__BaseUrl (Reservation)
- secret values placeholders for Postgres connection string (do not commit real secrets)

## Constraints
- Keep secrets out of Git.
- Use `--set` or external secret files for real connection string.
- Maintain stable resource names for DNS.

## Acceptance Criteria
- `helm install stationservice ./charts/stationservice -n ev-platform-dev` succeeds
- `helm install reservationservice ./charts/reservationservice -n ev-platform-dev` succeeds
- Both services become Ready
- Ingress (if included) works OR remains handled by separate chart (document choice)

## Implementation Steps
1. Create chart skeletons.
2. Migrate YAML into templates.
3. Add helpers for names/labels.
4. Parameterize:
   - replicas
   - resources
   - env vars
5. Provide install/upgrade/uninstall commands in `charts/README.md`.

## Verification
- `helm lint` passes
- `helm template` produces expected YAML
- Deploy and run smoke test (create station + reservation)

## Stop Condition
Stop after Helm deployments match raw YAML behavior.
