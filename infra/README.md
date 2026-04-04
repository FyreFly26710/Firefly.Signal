# Firefly Signal Infrastructure

This folder contains the tracked infrastructure assets that are intentionally shared in the repository.

## Contents
- `docker/`
  - production Dockerfiles for backend APIs
- `deploy/`
  - production Docker Compose and sample environment contract

## Environment Model
- local backend infrastructure for development stays in `services/api/docker-compose.yml`
- backend production packaging lives in `infra/docker/`
- backend production runtime composition lives in `infra/deploy/`

## Main Rules
- keep dev and prod deployment concerns separate
- keep local backend iteration optimized for VS Code and `dotnet run`
- keep production Dockerfiles outside the API project folders
- keep machine-specific deployment runbooks and secrets out of the tracked repo
