# Phase 3 – Task 06: Kubernetes Service Discovery (Reservation → Station via DNS)

## Task Type
Feature

## Context
In Kubernetes, ReservationService must call StationService via Cluster DNS, not IP addresses.
This ensures portability and supports scaling.

## Objective
Configure ReservationService inside K8s to call StationService using the Kubernetes Service name.

## Deliverables
- Update ReservationService ConfigMap:
  - `Services__StationService__BaseUrl=http://stationservice-api.ev-platform-dev.svc.cluster.local`
  - or simply `http://stationservice-api` if same namespace (document choice)
- Add timeout configuration for HttpClient if not already present
- Ensure failure mode is clear:
  - If StationService unreachable -> return 503/502 with ProblemDetails

## Constraints
- Do not introduce API Gateway yet.
- Do not hardcode IPs.
- Keep the abstraction interface (IStationDirectoryClient) intact.

## Acceptance Criteria
- Inside K8s, reservation creation validates station existence successfully
- If StationService is scaled to 0 or deleted, ReservationService returns a clear error

## Implementation Steps
1. Update ReservationService ConfigMap.
2. Restart deployment:
   - `kubectl rollout restart deployment/reservationservice-api -n ev-platform-dev`
3. Validate DNS:
   - (optional) ephemeral pod curl to `stationservice-api:8080/health/live`
4. Perform end-to-end test:
   - Create station
   - Create reservation

## Verification
- Show output of:
  - `kubectl get svc -n ev-platform-dev`
  - `kubectl logs -n ev-platform-dev deployment/reservationservice-api`

## Stop Condition
Stop after ReservationService successfully calls StationService inside the cluster.
