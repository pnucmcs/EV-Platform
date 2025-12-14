# Task — Reservation → Station validation via HTTP (Phase 2)

## Task title
Validate Station existence in Reservation Service via HTTP

## Task type
Spike / Feature

## Context
Reservation Service must not accept reservations for non-existent stations. In Phase 2 we validate by calling Station Service over HTTP behind an interface, so we can replace it with event-driven validation later.

## Objective
Implement synchronous station existence validation in Reservation Service using a typed HttpClient and an abstraction interface.

## Deliverables
- Application interface: `IStationDirectoryClient`
- Infrastructure typed HttpClient implementation
- Config key: `Services:StationService:BaseUrl`
- Handler updated to validate StationId before writing reservation

## Constraints
- Keep behind interface (swap later to event-driven)
- Use timeouts and basic error handling
- Do NOT add messaging here

## Acceptance criteria
- Invalid StationId → 404 (or 400 with clear ProblemDetails)
- Valid StationId → reservation created successfully

## Stop condition
Stop after validation works end-to-end with both services running.
