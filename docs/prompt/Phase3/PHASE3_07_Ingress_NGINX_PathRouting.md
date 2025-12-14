# Phase 3 – Task 07: Ingress (NGINX) – Path-Based Routing to Both Services

## Task Type
Feature / Platform Enablement

## Context
We need a single external entry point for local network access (later this becomes API gateway/BFF or remains path routing).
Ingress must expose:
- StationService
- ReservationService

## Objective
Expose both services externally via NGINX Ingress using path routing.

## Deliverables
- `k8s/50-ingress.yaml` (or Helm values later)
- Ingress routes:
  - `/station` -> stationservice-api
  - `/reservation` -> reservationservice-api
- Optional rewrite rules (if controllers expect `/api/v1/...`):
  - Document exact behavior
- Document access URL pattern in `docs/access.md`

## Constraints
- No TLS requirement for now (can add later).
- Avoid hostnames that require DNS changes unless you document /etc/hosts option.
- Must work in your LAN environment.

## Acceptance Criteria
- From your LAN machine:
  - Hitting Ingress address reaches each service
  - Swagger works for each (in Development)
- No port-forward required for normal access

## Implementation Steps
1. Confirm NGINX Ingress controller installed (if not, install via Helm as a prerequisite step).
2. Create Ingress resource in `ev-platform-dev`.
3. Determine Ingress IP/NodePort and document it.
4. Test access:
   - `http://<ingress-ip>/station/swagger`
   - `http://<ingress-ip>/reservation/swagger`
   (or your chosen paths)

## Verification
- Provide:
  - `kubectl get ingress -n ev-platform-dev`
  - `kubectl describe ingress ...`
  - screenshots optional (not required)

## Stop Condition
Stop after Ingress routing works for both services.
