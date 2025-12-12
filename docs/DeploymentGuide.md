# RabbitMQ Deployment Guide

## Overview
This document provides step‑by‑step instructions for deploying a RabbitMQ cluster on a Kubernetes cluster using the Bitnami Helm chart. The guide covers prerequisites, installation, configuration, and verification.

## Prerequisites
- A Kubernetes cluster (v1.24+ recommended).
- `kubectl` configured to talk to the cluster.
- Helm 3 installed.
- A storage class that supports dynamic provisioning (e.g., `local-storage`, `gp2`, `standard`).

## 1. Add the Bitnami Helm Repository
```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
```

## 2. Create a Namespace (optional)
```bash
kubectl create namespace rabbitmq
```

## 3. Install RabbitMQ with Helm
```bash
helm install rabbitmq bitnami/rabbitmq \
  --namespace rabbitmq \
  --set replicaCount=3 \
  --set persistence.enabled=true \
  --set persistence.storageClass=local-storage \
  --set persistence.size=10Gi \
  --set auth.username=admin \
  --set auth.password=supersecret \
  --set auth.erlangCookie=supersecretcookie
```

### Explanation of key values
- `replicaCount=3` – creates a 3‑node cluster for high availability.
- `persistence.enabled=true` – ensures data survives pod restarts.
- `persistence.storageClass` – replace with your cluster’s storage class.
- `persistence.size` – size of each node’s persistent volume.
- `auth.username/password` – credentials for the `admin` user.
- `auth.erlangCookie` – shared cookie for clustering; keep it secret.

## 4. Verify the Deployment
```bash
# Check pods
kubectl get pods -n rabbitmq

# Check services
kubectl get svc -n rabbitmq
```

All three pods should be in `Running` state. The `rabbitmq` service will expose port 5672 (AMQP) and 15672 (management UI).

## 5. Access the Management UI
```bash
# Port‑forward the service
kubectl port-forward svc/rabbitmq 15672:15672 -n rabbitmq
```
Open `http://localhost:15672` in a browser and log in with the credentials you set.

## 6. Expose RabbitMQ Externally (Optional)
If you need external access, create an Ingress or LoadBalancer service:
```bash
kubectl expose svc rabbitmq --type=LoadBalancer --name=rabbitmq-lb -n rabbitmq
```

## 7. Monitoring & Metrics
The Bitnami chart includes a Prometheus exporter. Add the following to your `values.yaml` or via `--set`:
```yaml
metrics:
  enabled: true
  serviceMonitor:
    enabled: true
```
Prometheus will scrape metrics at `/metrics`.

## 8. Backup & Restore
Use the built‑in `rabbitmqctl snapshot` command inside a pod:
```bash
kubectl exec -it $(kubectl get pod -n rabbitmq -l app.kubernetes.io/name=rabbitmq -o jsonpath='{.items[0].metadata.name}') -n rabbitmq -- rabbitmqctl snapshot my_snapshot
```
Restore with `rabbitmqctl restore my_snapshot`.

## 9. Clean Up
```bash
helm uninstall rabbitmq -n rabbitmq
kubectl delete namespace rabbitmq
```

---

Feel free to adapt the values to match your environment. This guide is intended as a reference for future deployments.
