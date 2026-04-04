# Deployment Overview

This document defines the intended deployment model for Firefly Signal.

## Frontend
- platform: Cloudflare Pages
- source: `apps/web`
- build command: `npm run build`
- output directory: `dist`
- routing: SPA fallback via `_redirects`

## Backend
- image registry: Docker Hub
- runtime host: Mac mini
- remote exposure: Cloudflare Tunnel
- deployment method: GitHub Actions publishes images, then SSHes into the Mac mini and runs a production Compose update

## Local Development
- local infra services run from [services/api/docker-compose.yml](D:\projects\Firefly%20Signal\services\api\docker-compose.yml)
- APIs run locally via VS Code launch profiles or `dotnet run`
- web app runs locally via Vite

## Production Packaging
- production backend Dockerfiles live in `infra/docker/`
- production backend compose lives in `infra/deploy/docker-compose.production.yml`
- production env template lives in `infra/deploy/.env.example`

## Required GitHub Secrets

### Cloudflare Pages
- `CLOUDFLARE_API_TOKEN`
- `CLOUDFLARE_ACCOUNT_ID`
- `CLOUDFLARE_PAGES_PROJECT_NAME`

### Docker Hub
- `DOCKERHUB_USERNAME`
- `DOCKERHUB_TOKEN`
- `DOCKERHUB_NAMESPACE`

### Mac mini deployment
- `DEPLOY_SSH_HOST`
- `DEPLOY_SSH_USERNAME`
- `DEPLOY_SSH_PRIVATE_KEY`
- `DEPLOY_REMOTE_PATH`
- `JWT_SIGNING_KEY`

### Backend runtime configuration
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `RABBITMQ_DEFAULT_USER`
- `RABBITMQ_DEFAULT_PASS`

## Current Recommendation
- keep the frontend and backend release workflows separate
- build and push one image per backend API
- keep the production host update step simple: rewrite `.env`, pull, up, remove orphans
