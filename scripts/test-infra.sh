#!/usr/bin/env bash
set -euo pipefail

# Simple connectivity checks for external infra. Adjust hosts/ports as needed.
# Usage: ./scripts/test-infra.sh

PG1_HOST=${PG1_HOST:-192.168.1.191}
PG1_PORT=${PG1_PORT:-5432}
PG1_USER=${PG1_USER:-admin}
PG1_PASS=${PG1_PASS:-admin}
PG1_DB=${PG1_DB:-postgres}

PROM_HOST=${PROM_HOST:-192.168.1.191}
PROM_PORT=${PROM_PORT:-30104}

REDIS_HOST=${REDIS_HOST:-192.168.1.191}
REDIS_PORT=${REDIS_PORT:-30059}

GRAFANA_HOST=${GRAFANA_HOST:-192.168.1.191}
GRAFANA_PORT=${GRAFANA_PORT:-30037}

TIMEOUT=${TIMEOUT:-5}

check_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing dependency: $1" >&2
    exit 1
  fi
}

check_psql() {
  local host=$1 port=$2 user=$3 pass=$4 db=$5 label=$6
  echo "Checking Postgres (${label}) at ${host}:${port}..."
  PGPASSWORD="$pass" psql -h "$host" -p "$port" -U "$user" -d "$db" -c "select version();" -v ON_ERROR_STOP=1 >/dev/null
  echo "Postgres (${label}) OK"
}

check_redis() {
  local host=$1 port=$2
  echo "Checking Redis at ${host}:${port}..."
  redis-cli -h "$host" -p "$port" ping >/dev/null
  echo "Redis OK"
}

check_grafana() {
  local host=$1 port=$2
  echo "Checking Grafana at http://${host}:${port}/login..."
  local code
  code=$(curl -s -o /dev/null -w "%{http_code}" --max-time "$TIMEOUT" "http://${host}:${port}/login")
  if [[ "$code" =~ ^2|3 ]]; then
    echo "Grafana OK (HTTP ${code})"
  else
    echo "Grafana check failed (HTTP ${code})" >&2
    exit 1
  fi
}

check_prometheus() {
  local host=$1 port=$2
  echo "Checking Prometheus at http://${host}:${port}/-/ready..."
  local code
  code=$(curl -s -o /dev/null -w "%{http_code}" --max-time "$TIMEOUT" "http://${host}:${port}/-/ready")
  if [[ "$code" =~ ^2|3 ]]; then
    echo "Prometheus OK (HTTP ${code})"
  else
    echo "Prometheus check failed (HTTP ${code})" >&2
    exit 1
  fi
}

check_k8s() {
  echo "Checking Kubernetes context..."
  kubectl cluster-info >/dev/null
  kubectl get nodes -o wide
  echo "Kubernetes OK"
}

# New function to check RabbitMQ cluster on Kubernetes
check_rabbitmq() {
  local namespace=${1:-rabbitmq}
  echo "Checking RabbitMQ cluster in namespace $namespace..."
  # Ensure the namespace exists
  if ! kubectl get namespace "$namespace" >/dev/null 2>&1; then
    echo "Namespace $namespace does not exist" >&2
    exit 1
  fi
  # Get all RabbitMQ pods
  local pods
  pods=$(kubectl get pods -n "$namespace" -l app.kubernetes.io/name=rabbitmq -o jsonpath='{.items[*].metadata.name}')
  if [[ -z "$pods" ]]; then
    echo "No RabbitMQ pods found in namespace $namespace" >&2
    exit 1
  fi
  for pod in $pods; do
    local status
    status=$(kubectl get pod "$pod" -n "$namespace" -o jsonpath='{.status.phase}')
    if [[ "$status" != "Running" ]]; then
      echo "RabbitMQ pod $pod is not running (status=$status)" >&2
      exit 1
    fi
  done
  echo "All RabbitMQ pods are running."
  # Check the service exists
  if ! kubectl get svc rabbitmq -n "$namespace" >/dev/null 2>&1; then
    echo "RabbitMQ service not found in namespace $namespace" >&2
    exit 1
  fi
  echo "RabbitMQ service is available in namespace $namespace."
}

main() {
  check_cmd psql
  check_cmd redis-cli
  check_cmd curl
  check_cmd kubectl

  check_psql "$PG1_HOST" "$PG1_PORT" "$PG1_USER" "$PG1_PASS" "$PG1_DB" "primary"
  check_redis "$REDIS_HOST" "$REDIS_PORT"
  check_grafana "$GRAFANA_HOST" "$GRAFANA_PORT"
  check_prometheus "$PROM_HOST" "$PROM_PORT"
  check_k8s
  # New check for RabbitMQ
  check_rabbitmq

  echo "All checks passed."
}

main "$@"
