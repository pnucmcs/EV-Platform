# Phase 6 – Task 03: BillingService (Scaffold + Invoice/Ledger + APIs)

## Task Type
Feature

## Context
BillingService will produce invoices when sessions complete.
Real payment processing is deferred; we implement billing records and payment status lifecycle.

## Objective
Create BillingService with:
- invoice generation logic
- payment status tracking (stub)
- event consumption readiness for `session.completed.v1`

## Deliverables
1. Solution and layering:
   - BillingService.Api / Application / Domain / Infrastructure
2. Domain model:
   - Invoice (id, sessionId, userId, stationId, amount, currency, status, createdAtUtc)
   - PaymentAttempt (id, invoiceId, status, provider, createdAtUtc, lastError)
   - Status enums: InvoiceStatus (Pending, Paid, Failed, Cancelled), PaymentStatus (Initiated, Authorized, Captured, Failed)
3. APIs:
   - `GET /api/v1/invoices/{id}`
   - `GET /api/v1/users/{userId}/invoices`
   - (optional) `POST /api/v1/invoices/{id}/mark-paid` (admin/testing only)
4. Event consumer placeholder:
   - handler for `session.completed.v1` (can be stubbed until Task 05)
5. Helm chart

## Constraints
- No integration with Stripe/Adyen yet.
- Billing must be idempotent for session completion (same session -> same invoice).

## Acceptance Criteria
- Invoices persist in Postgres.
- APIs work via Swagger.
- Service deploys to K8s via Helm.

## Stop Condition
Stop after BillingService is deployed and invoice APIs work.
