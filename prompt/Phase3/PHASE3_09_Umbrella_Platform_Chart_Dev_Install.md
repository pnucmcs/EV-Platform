# Phase 3 – Task 09: Umbrella “Platform” Chart (Deploy Both Services Together)

## Task Type
Feature / Release Simplification

## Context
As services grow, you will want one command to deploy “the platform baseline.”
An umbrella chart provides that without forcing a monorepo or a single release artifact.

## Objective
Create a top-level Helm chart that deploys:
- StationService
- ReservationService
(and later additional services as dependencies)

## Deliverables
- `charts/ev-platform/`:
  - Chart.yaml with dependencies on:
    - stationservice
    - reservationservice
  - values.yaml that can pass subchart values
- `values-dev.yaml` for `ev-platform` chart

## Constraints
- Do not embed secrets.
- Keep dependency versions pinned.
- Maintain environment namespace separation (dev now, staging/prod later).

## Acceptance Criteria
- `helm install ev-platform ./charts/ev-platform -n ev-platform-dev` deploys both services
- `helm upgrade` updates image tags cleanly
- `helm uninstall` removes both services

## Implementation Steps
1. Create umbrella chart.
2. Add dependencies (local file path or packaged chart strategy; document).
3. Expose configuration pass-through:
   - stationservice.image.tag
   - reservationservice.env.Services__StationService__BaseUrl, etc.
4. Provide runbook in `charts/ev-platform/README.md`.

## Verification
- Fresh install into empty namespace works.
- Reinstall works after uninstall.

## Stop Condition
Stop after umbrella chart can deploy both services reliably.
