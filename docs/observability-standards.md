# Observability Standards (Metrics, Tracing, Logging)

## Naming and labels
- Service identity: `service.name`, `service.version`, `deployment.environment`
- Correlation: `trace_id`/`span_id` plus `X-Correlation-ID` echoed in logs and responses
- Metrics prefix: `ev_platform_*` where applicable; avoid high-cardinality labels (no userId/deviceId)
- HTTP spans: `HTTP {method}` with attributes `http.route`, `http.status_code`
- Messaging spans: `messaging.system=rabbitmq`, `messaging.destination=ev.platform`, `messaging.rabbitmq.routing_key`

## Required instrumentation (baseline)
- HTTP server and client instrumentation (ASP.NET Core + HttpClient)
- EF Core instrumentation
- Resource attributes set from config (service name/version/environment)
- OTLP exporter configurable; Prometheus exporter enabled to expose `/metrics` for scraping

## Logging
- JSON logs
- Include `CorrelationId` and `TraceId` (Serilog output template already includes CorrelationId; TraceId available via OTEL)
- No PII, no secrets

## Messaging traces
- RabbitMQ publish/consume spans use ActivitySource `Ev.Messaging` and propagate W3C `traceparent`/`tracestate` in message headers.
- Publishers set `messaging.*` attributes including routing key and exchange; consumers start child spans from extracted context.
- Ensure the tracer provider registers `AddSource("Ev.Messaging")` to see the messaging hops linked to the HTTP parent trace.

## Configuration (appsettings)
```json
"OpenTelemetry": {
  "Enabled": true,
  "Exporter": "otlp",
    "Otlp": {
      "Endpoint": "http://192.168.1.191:4317"
    },
    "Metrics": {
      "Prometheus": {
        "ScrapeEndpoint": "/metrics"
      }
    },
    "Tracing": {
      "Sampling": "parentbased_always_on"
    }
}
```

Disable by setting `OpenTelemetry:Enabled=false`.

## Versioning
- Keep instrumentation consistent across services; add new metrics/spans deliberately.
- Avoid custom metrics unless needed; prefer built-in HTTP/EF.
