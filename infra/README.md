# Firefly Signal Infrastructure

This folder is the deployment and environment source of truth for Firefly Signal.

## Contents
- `docs/`
  - deployment planning and operational guidance
- `docker/`
  - production Dockerfiles for backend APIs
- `deploy/`
  - production Docker Compose and remote deployment assets
- `cloudflare/pages/`
  - Cloudflare Pages deployment notes
- `cloudflare/tunnels/`
  - Cloudflare Tunnel setup notes for the Mac mini

## Environment Model
- local backend infrastructure for development stays in `services/api/docker-compose.yml`
- backend production packaging lives in `infra/docker/`
- backend production runtime composition lives in `infra/deploy/`
- frontend production delivery targets Cloudflare Pages

## Main Rules
- keep dev and prod deployment concerns separate
- keep local backend iteration optimized for VS Code and `dotnet run`
- keep production Dockerfiles outside the API project folders
- treat this folder as the deployable operations surface for the repo
