# Deployment Assets

This folder contains the backend production deployment assets for the Mac mini.

## Files
- `docker-compose.production.yml`
- `.env.example`

## Intended Flow
1. GitHub Actions builds and pushes backend images to Docker Hub.
2. GitHub Actions connects to the Mac mini over SSH.
3. The workflow refreshes the production compose file and env file on the host.
4. The host runs `docker compose pull` and `docker compose up -d --remove-orphans`.

## Host Expectations
- Docker and Docker Compose installed
- a deployment directory matching `DEPLOY_REMOTE_PATH`
- SSH access available from GitHub Actions
- Cloudflare Tunnel configured separately for public exposure

This setup is intentionally simple and can be hardened later once the Mac mini is prepared.
