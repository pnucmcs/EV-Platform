# Task List

- [ ] Align EF Core package versions (8.0.8)
- [ ] Add DbContext, Repository, and Migrations for Billing, User, Pricing, Auth
- [ ] Implement RabbitMQ consumers/publishers for ReservationCreated, PaymentCompleted, StationStatusChanged
- [ ] Create Dockerfiles for each service
- [ ] Create docker‑compose.yml to launch Postgres, Mongo, Redis, RabbitMQ locally
- [ ] Add Helm charts for all services
- [ ] Implement routing logic in the Gateway API
- [ ] Switch authentication to JWT (hashing, refresh tokens, validation)
- [ ] Wire OpenTelemetry, Prometheus, Loki, Grafana
- [ ] Add GitHub Actions workflows (build, lint, test, deploy)
- [ ] Update System.Text.Json to a non‑vulnerable version
- [ ] Remove or replace any unused/abandoned files