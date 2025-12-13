# Grafana Dashboards – EV Platform

This document captures reusable dashboards/queries for Grafana. Import the JSON snippets below (or build them manually) and set data sources/variables per environment.

## Data sources
- `Prometheus`: points to cluster Prometheus scraping `/metrics` on each service.
- (Optional) `Loki`/`OTLP` traces: if available, wire panels to logs/traces.

## Templating variables
- `namespace`: default `ev-platform-dev`
- `service`: regex against `service_name` label (e.g., `reservation-service|station-service|notification-service`)
- `instance`: scrape target (pod IP:port)

## Dashboard 1: Service Health (per service)
Panels (PromQL):
- **Request rate (RPS)**  
  `sum by(service_name) (rate(http_server_request_duration_seconds_count{namespace="$namespace",service_name=~"$service"}[5m]))`
- **Error rate (5xx)**  
  `sum by(service_name) (rate(http_server_request_duration_seconds_count{namespace="$namespace",service_name=~"$service",http_status_code=~"5.."}[5m]))`
- **p95 latency**  
  `histogram_quantile(0.95, sum by(le, service_name) (rate(http_server_request_duration_seconds_bucket{namespace="$namespace",service_name=~"$service"}[5m])))`
- **Active requests**  
  `sum by(service_name) (http_server_active_requests{namespace="$namespace",service_name=~"$service"})`
- **Health endpoint status** (stat panel)  
  `max by(service_name) (up{job=~".*${service}.*", namespace="$namespace"})`

## Dashboard 2: Infrastructure (Pods)
Panels (PromQL, assumes kube-state-metrics/node-exporter scraped):
- **CPU usage (mCPU)**  
  `sum by(pod) (rate(container_cpu_usage_seconds_total{namespace="$namespace",pod=~".*"}[5m])) * 1000`
- **Memory usage (MiB)**  
  `sum by(pod) (container_memory_working_set_bytes{namespace="$namespace",pod=~".*"}) / 1024 / 1024`
- **Pod restarts**  
  `max by(pod) (kube_pod_container_status_restarts_total{namespace="$namespace"})`
- **Pod status** (table)  
  `kube_pod_status_phase{namespace="$namespace"}`

## Dashboard 3: Eventing (RabbitMQ + Outbox)
Panels (PromQL; requires RabbitMQ exporter and outbox gauges/counters):
- **Messages published (rate by routing key)**  
  `sum by(routing_key) (rate(rabbitmq_queue_messages_published_total{namespace="$namespace",queue=~".*notification.*"}[5m]))`
- **Messages consumed (rate by queue)**  
  `sum by(queue) (rate(rabbitmq_queue_consumed_total{namespace="$namespace"}[5m]))`
- **Outbox backlog (station)**  
  `sum(outbox_pending_count{service="station-service",namespace="$namespace"})`
- **Outbox backlog (reservation)**  
  `sum(outbox_pending_count{service="reservation-service",namespace="$namespace"})`
- **Dispatcher errors**  
  `sum(rate(outbox_dispatch_errors_total{namespace="$namespace"}[5m]))`

If RabbitMQ exporter metrics differ, adjust metric names accordingly (e.g., `rabbitmq_queue_messages_ready`, `rabbitmq_queue_consumers`).

## Import/export
- Use Grafana “Import dashboard” and paste the JSON generated from these panels. Save dashboards as:
  - `EV Platform / Service Health`
  - `EV Platform / Infrastructure`
  - `EV Platform / Eventing`
- Mark as “folder: EV Platform” so they’re grouped.

## Reusability
- Rely on labels (`namespace`, `service_name`, `instance`, `routing_key`) rather than hard-coded hostnames. Switching environments requires only variable defaults.
- Do not bake credentials into dashboards; use existing Prometheus datasource auth.

## Validation checklist
- Panels return data in `ev-platform-dev`.
- Switching `service` variable shows per-service slices.
- Outbox backlog rises when RabbitMQ is down; drops after recovery.
- Error rate spikes on forced 500s and recovers when fixed.
