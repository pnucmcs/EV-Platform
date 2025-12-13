# Phase 4 – Event Flow Integration Runbook

Step-by-step validation of HTTP → Outbox → RabbitMQ → Consumer in `ev-platform-dev`.

## 1) Pre-checks
- Namespace exists: `kubectl get ns ev-platform-dev`
- Pods healthy: `kubectl get pods -n ev-platform-dev`
- Postgres reachable from cluster (optional exec):  
  `kubectl exec -it -n ev-platform-dev deploy/stationservice -- pg_isready -h 192.168.1.191 -p 5432`
- RabbitMQ reachable (management optional): ensure RabbitMQ pod/service is Running.

## 2) RabbitMQ topology check
```bash
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_exchanges
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_queues
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_bindings
```
Expect exchange `ev.platform`, queues `notification-service.events` (and others), bindings for `station.*.v1`, `reservation.*.v1`, `session.*.v1`.

## 3) Trigger actions (HTTP)
Port-forward APIs:
```bash
kubectl port-forward -n ev-platform-dev svc/stationservice-api 8080:80
kubectl port-forward -n ev-platform-dev svc/reservationservice-api 8081:80
kubectl port-forward -n ev-platform-dev svc/notificationservice-api 8082:80
```
Create station:
```bash
curl -X POST http://localhost:8080/api/v1/stations \
  -H "Content-Type: application/json" \
  -d '{"name":"Main St","latitude":40.0,"longitude":-73.9}'
```
Update status:
```bash
curl -X PUT http://localhost:8080/api/v1/stations/{stationId} \
  -H "Content-Type: application/json" \
  -d '{"name":"Main St","latitude":40.0,"longitude":-73.9,"status":2}'
```
Create reservation:
```bash
curl -X POST http://localhost:8081/api/v1/reservations \
  -H "Content-Type: application/json" \
  -d '{"userId":"<guid>","stationId":"<station-guid>","startsAtUtc":"2025-01-01T10:00:00Z","endsAtUtc":"2025-01-01T11:00:00Z"}'
```
Start session:
```bash
curl -X POST http://localhost:8081/api/v1/sessions \
  -H "Content-Type: application/json" \
  -d '{"reservationId":"<reservation-guid>","stationId":"<station-guid>","chargerId":null,"startedAtUtc":"2025-01-01T10:05:00Z"}'
```
Complete session:
```bash
curl -X PATCH http://localhost:8081/api/v1/sessions/{sessionId} \
  -H "Content-Type: application/json" \
  -d '{"status":2,"endedAtUtc":"2025-01-01T10:45:00Z"}'
```

## 4) Verify Outbox persistence
Station outbox:
```bash
kubectl exec -it -n ev-platform-dev deploy/stationservice -- psql "$ConnectionStrings__Postgres" -c "select id,routing_key,processed_at_utc,publish_attempts from outbox_messages order by occurred_at_utc desc limit 10;"
```
Reservation outbox:
```bash
kubectl exec -it -n ev-platform-dev deploy/reservationservice -- psql "$ConnectionStrings__Postgres" -c "select id,routing_key,processed_at_utc,publish_attempts from outbox_messages order by occurred_at_utc desc limit 10;"
```
Expect new rows with `processed_at_utc` set after dispatcher runs.

## 5) Verify RabbitMQ queues
```bash
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_queues name messages_ready messages_unacknowledged
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_bindings | grep ev.platform
```
Queues should have messages briefly; they should drain after NotificationService consumes.

## 6) Verify consumer logs (NotificationService)
```bash
kubectl logs -n ev-platform-dev deploy/notificationservice
```
Expect log lines with `eventType`, `routingKey`, `eventId`, `correlationId`.

## 7) RabbitMQ down/up resilience test
1. Scale RabbitMQ to 0:
```bash
kubectl scale deploy/rabbitmq -n ev-platform-dev --replicas=0
```
2. Trigger create station/reservation as above.
3. Check outbox: rows should remain with `processed_at_utc` null.
4. Scale RabbitMQ back:
```bash
kubectl scale deploy/rabbitmq -n ev-platform-dev --replicas=1
```
5. Wait for dispatcher to run; check outbox rows now have `processed_at_utc` set; check consumer logs for received events.

## 8) Troubleshooting
- If outbox not draining: check dispatcher logs in station/reservation pods and RabbitMQ connectivity.
- If queues empty but outbox filled: verify exchange name (`ev.platform`) and routing keys in bindings match `station.created.v1`, `station.status_changed.v1`, `reservation.*.v1`, `session.*.v1`.
- If consumer not receiving: confirm NotificationService queue exists and bindings present; check consumer pod logs for errors.
- For HTTP failures: check pod logs (`kubectl logs deploy/<service> -n ev-platform-dev`) and health endpoints (`/health/live`, `/health/ready`).
