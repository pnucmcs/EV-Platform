# Accessing Services via Ingress (NGINX)

Assuming the NGINX Ingress controller is installed and the Ingress in `k8s/50-ingress.yaml` is applied to the `ev-platform-dev` namespace:

- StationService: `http://<ingress-ip>/station`
- ReservationService: `http://<ingress-ip>/reservation`

Swagger (Development only):
- `http://<ingress-ip>/station/swagger`
- `http://<ingress-ip>/reservation/swagger`

Rewrite behavior:
- Paths `/station/...` are forwarded to StationService with the `/station` prefix stripped (e.g., `/station/api/v1/stations` -> `/api/v1/stations`).
- Paths `/reservation/...` are forwarded to ReservationService with the `/reservation` prefix stripped.

To discover the Ingress IP:
```bash
kubectl get ingress -n ev-platform-dev
```

If you need a fixed host name, add an `/etc/hosts` entry pointing to the Ingress IP, but host-based routing is not required for path routing. TLS is not configured yet. 
