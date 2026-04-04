# Backend Deployment Assets

This folder contains the public, shareable backend deployment assets for Firefly Signal.
It documents the deployment contract without storing environment-specific values.


## Files
- `docker-compose.production.yml`
- `.env.example`

## Target Runtime Model
- Backend images are built in GitHub Actions and pushed to Docker Hub.
- The Mac mini runs the production stack with Docker Compose.
- The public API is exposed through Cloudflare Tunnel.
- The frontend is deployed separately to Cloudflare Pages.

## CI/CD Flow
1. `Release Backend` builds and pushes one image per backend service.
2. The same workflow connects to the Mac mini over SSH.
3. The workflow uploads `docker-compose.production.yml`.
4. The workflow writes a fresh `.env` file on the host from GitHub Secrets.
5. The host runs `docker compose pull`.
6. The host runs `docker compose up -d --remove-orphans`.

The frontend is handled by `.github/workflows/deploy-frontend-pages.yml`, not by this folder.

## Required GitHub Secrets

### Docker Hub
- `DOCKERHUB_USERNAME`
- `DOCKERHUB_TOKEN`
- `DOCKERHUB_NAMESPACE`

### Backend Deploy
- `DEPLOY_SSH_HOST`
- `DEPLOY_SSH_PORT`
- `DEPLOY_SSH_USERNAME`
- `DEPLOY_SSH_PRIVATE_KEY`
- `DEPLOY_REMOTE_PATH`
- `JWT_SIGNING_KEY`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `RABBITMQ_DEFAULT_USER`
- `RABBITMQ_DEFAULT_PASS`

### Frontend Deploy
- `CLOUDFLARE_API_TOKEN`
- `CLOUDFLARE_ACCOUNT_ID`
- `CLOUDFLARE_PAGES_PROJECT_NAME`

## Host Prerequisites
- Docker and `docker compose` available on the Mac mini
- a deployment directory matching `DEPLOY_REMOTE_PATH`
- SSH access available from GitHub Actions
- Cloudflare Tunnel configured for the gateway hostname
- direct SSH access available on the host and router-forwarded port
- deploy host reachable from GitHub Actions with standard `ssh` and `scp`

Recommended hostnames for the current Firefly Signal setup:
- backend API: `api.signal.firefly-ai.co.uk`
- deploy SSH: `ssh.firefly-ai.co.uk` or another direct SSH hostname

## Compose Contract
`docker-compose.production.yml` expects these runtime values:
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `RABBITMQ_DEFAULT_USER`
- `RABBITMQ_DEFAULT_PASS`
- `JWT_SIGNING_KEY`
- `DOCKERHUB_NAMESPACE`
- `IMAGE_TAG`

The backend stack currently includes:
- PostgreSQL
- pgweb
- Redis
- RedisInsight
- RabbitMQ
- identity API
- job search API
- AI API
- gateway API

## Important Networking Note
The repository tunnel docs assume Cloudflare Tunnel forwards to `http://localhost:21000` on the Mac mini host.
The current compose file sets `ASPNETCORE_HTTP_PORTS=8080` inside the `gateway-api` container, but it does not publish that port to the host.

That means one of these needs to be true in the target environment:
- the gateway container is published to the host, for example with `21000:8080`, or
- `cloudflared` runs inside Docker on the same network and talks to `http://gateway-api:8080`

Keep this aligned before relying on the tunnel setup.

## First-Time Bring-Up
1. Populate the GitHub secrets required by the backend and frontend workflows.
2. Prepare the Mac mini deployment directory.
3. Set up Cloudflare Tunnel for the API hostname.
4. Make sure the tunnel can reach the gateway target URL.
5. Trigger the backend workflow manually once.
6. Trigger the frontend Pages workflow manually once.

## Verification Commands On The Mac Mini
```bash
cd "$DEPLOY_REMOTE_PATH"
docker compose -f docker-compose.production.yml --env-file .env ps
docker compose -f docker-compose.production.yml --env-file .env logs gateway-api --tail=200
docker compose -f docker-compose.production.yml --env-file .env logs identity-api --tail=200
docker compose -f docker-compose.production.yml --env-file .env logs job-search-api --tail=200
docker compose -f docker-compose.production.yml --env-file .env logs ai-api --tail=200
```
