# Phase 1 — Local Dev Environment Setup

## Tools to Install (Ubuntu/WSL)

1. .NET 8 SDK: `sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0` (or use Microsoft package feed).
2. Docker Engine + Compose: install Docker CE, add your user to `docker` group.
3. kubectl: `curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"`; chmod + mv to `/usr/local/bin`.
4. Helm: `curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash`.
5. Optional: `make`, `jq`, `kubectx/kubens`, `k9s`.

## Verify Commands

Run these and ensure they succeed:

```bash
dotnet --version
docker info
docker compose version
kubectl version --client
helm version
```

If you have a cluster context set, also run:

```bash
kubectl get nodes
```

## Local Solution Skeleton (next step after tools verified)

We will create the solution and service folders under `src/`:

```bash
mkdir -p src/services/{gateway,auth-service,user-service,station-service,reservation-service,billing-service,notification-service,telemetry-service,reporting-service} src/shared deploy/helm infra/.data docs
dotnet new sln -n EvPlatform
```

Each service will follow a layered structure (`Api`, `Application`, `Domain`, `Infrastructure`) with shared libraries in `src/shared` for messaging and observability.
