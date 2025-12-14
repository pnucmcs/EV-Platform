# Phase 3 – Task 05: Kubernetes Baseline (Namespace + Secrets + ConfigMaps + Deployments) – Raw YAML First

## Task Type
Feature / Platform Enablement

## Context
Before Helm, we deploy using raw Kubernetes YAML to validate:
- networking
- DNS
- secrets/config patterns
- probes
- external Postgres connectivity from cluster

Kubernetes is running in a VM. RabbitMQ is inside K8s (not used yet in Phase 3 tasks).

## Objective
Deploy StationService and ReservationService into Kubernetes using raw YAML.

## Deliverables
In a `k8s/` folder (in each repo or a shared `deploy/` repo):
- `00-namespace.yaml` -> `ev-platform-dev`
- `10-stationservice-secret.yaml`
- `11-stationservice-configmap.yaml`
- `20-stationservice-deployment.yaml`
- `21-stationservice-service.yaml`
- `30-reservationservice-secret.yaml`
- `31-reservationservice-configmap.yaml`
- `40-reservationservice-deployment.yaml`
- `41-reservationservice-service.yaml`

Requirements:
- Deployments set:
  - requests/limits (basic)
  - liveness/readiness probes
- Services are ClusterIP
- Configuration uses env vars injected from ConfigMap/Secret

## Constraints
- Connection string MUST point to TrueNAS Postgres: `192.168.1.191:5432`
- DO NOT embed credentials in ConfigMap (use Secret).
- Do not expose externally yet (Ingress comes later in Task 07).

## Acceptance Criteria
- `kubectl apply -f k8s/` succeeds
- Pods reach Ready state
- `kubectl port-forward` to each service works
- StationService can persist to Postgres from inside K8s
- ReservationService can start and is healthy (even if Station URL not wired yet)

## Implementation Steps
1. Create namespace YAML.
2. Create Secrets:
   - `ConnectionStrings__Postgres` as a secret key
3. Create ConfigMaps:
   - `ASPNETCORE_ENVIRONMENT`
   - `Kestrel__Endpoints__Http__Url=http://0.0.0.0:8080`
   - `Services__StationService__BaseUrl` (Reservation only; can be placeholder until Task 06)
4. Create Deployments:
   - image placeholders `:latest` (will be refined later)
   - `containerPort: 8080`
   - probes:
     - live: `/health/live`
     - ready: `/health/ready`
5. Create Services:
   - stationservice-api
   - reservationservice-api

## Verification
- `kubectl get pods -n ev-platform-dev`
- `kubectl describe pod ...`
- Port-forward and hit:
  - `/health/live`
  - `/swagger` (if Development)
- Test create station/reservation via port-forward.

## Stop Condition
Stop after both services are deployed and reachable via port-forward.
