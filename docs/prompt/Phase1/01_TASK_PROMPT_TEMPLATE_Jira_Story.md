# Task Prompt Template — Jira Story Equivalent

We are working under the **Smart EV Charging Platform master prompt**.  
Do NOT redefine goals or infrastructure unless explicitly asked.

---

## Task title
[Short, concrete title]

Example: **Bootstrap Station Service .NET solution structure**

## Task type
- Feature / Spike / Refactor / Infra / Hardening

## Context
Explain where this task fits in the overall architecture.

Example:
This task establishes the Station Service baseline, which will later
be consumed by Reservation Service and emit events via RabbitMQ.

## Objective
What must exist at the end of this task.

Example:
- A compilable .NET 8 solution for Station Service
- Proper layering
- Swagger enabled
- No business logic yet

## Constraints
- Respect existing infra (TrueNAS + K8s)
- No shortcuts that block Kubernetes deployment later
- No unrelated features

## Deliverables
List tangible outputs:
- Files
- Projects
- APIs
- Configs

## Acceptance criteria
Bullet-point checks that define “done”.

## Implementation steps
You must provide:
1. Explanation (high level)
2. Exact commands
3. File structures
4. Code skeletons
5. Verification steps

## Stop condition
Explicitly state where to stop.

Example:
Stop after `dotnet build` succeeds and Swagger loads.  
Do NOT proceed to DB integration.
