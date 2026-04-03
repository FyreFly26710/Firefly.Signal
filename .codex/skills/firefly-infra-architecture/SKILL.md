---
name: firefly-infra-architecture
description: Design or refine deployment, Docker, Cloudflare Pages, GitHub Actions, or Mac mini runtime architecture in the Firefly Signal repository. Use when Codex is planning infrastructure or deployment conventions for this repo.
---

# Firefly Infrastructure Architecture

Read these files before making infrastructure recommendations:
- `AGENTS.md`
- `docs/plans.md`
- `infra/README.md`
- `infra/docs/deployment-overview.md`
- `infra/docs/local-development.md`

Preserve the repo's infrastructure direction:
- Cloudflare Pages for the frontend
- Docker Hub for backend images
- Mac mini as the backend runtime host
- Cloudflare Tunnel for backend exposure
- local backend infra through `services/api/docker-compose.yml`
- production backend Dockerfiles under `infra/docker/`

When proposing structure:
- keep dev and prod concerns separate
- keep local iteration friendly to VS Code
- keep production deployment simple and reviewable
- use one backend image per API
