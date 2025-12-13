# RabbitMQ Topology – EV Platform

We use a repeatable definitions-based topology (Helm value `loadDefinition.enabled=true` with `definitions.json`) to provision exchange, queues, and bindings.

## Exchange
- Name: `ev.platform`
- Type: `topic`
- Durable: true

## Queues (durable, non-auto-delete)
- `notification-service.events`
- `billing-service.events`
- `telemetry-service.events`

## Bindings
- `notification-service.events`
  - `station.*.v1`
  - `reservation.*.v1`
  - `session.*.v1`
- `billing-service.events`
  - `session.completed.v1`
- `telemetry-service.events`
  - `station.*.v1`
  - `reservation.*.v1`
  - `session.*.v1`

## definitions.json (example)
Place under your RabbitMQ Helm chart values (e.g., `rabbitmq.definitions`):
```json
{
  "users": [],
  "vhosts": [{ "name": "/" }],
  "permissions": [],
  "exchanges": [
    {
      "name": "ev.platform",
      "vhost": "/",
      "type": "topic",
      "durable": true,
      "auto_delete": false,
      "internal": false,
      "arguments": {}
    }
  ],
  "queues": [
    { "name": "notification-service.events", "vhost": "/", "durable": true, "auto_delete": false, "arguments": {} },
    { "name": "billing-service.events", "vhost": "/", "durable": true, "auto_delete": false, "arguments": {} },
    { "name": "telemetry-service.events", "vhost": "/", "durable": true, "auto_delete": false, "arguments": {} }
  ],
  "bindings": [
    { "source": "ev.platform", "vhost": "/", "destination": "notification-service.events", "destination_type": "queue", "routing_key": "station.*.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "notification-service.events", "destination_type": "queue", "routing_key": "reservation.*.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "notification-service.events", "destination_type": "queue", "routing_key": "session.*.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "billing-service.events", "destination_type": "queue", "routing_key": "session.completed.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "telemetry-service.events", "destination_type": "queue", "routing_key": "station.*.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "telemetry-service.events", "destination_type": "queue", "routing_key": "reservation.*.v1", "arguments": {} },
    { "source": "ev.platform", "vhost": "/", "destination": "telemetry-service.events", "destination_type": "queue", "routing_key": "session.*.v1", "arguments": {} }
  ]
}
```

## Apply via RabbitMQ Helm chart (bitnami example)
Set values (example):
```yaml
loadDefinition:
  enabled: true
  existingSecret: ""
  definitions: |-
    { ...copy the JSON above... }
```

Install/upgrade:
```bash
helm upgrade --install rabbitmq bitnami/rabbitmq -n ev-platform-dev -f rabbitmq-values.yaml
```

## Validation (commands)
```bash
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_exchanges
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_queues
kubectl exec -it -n ev-platform-dev deploy/rabbitmq -- rabbitmqctl list_bindings
```
