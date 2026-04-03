---
name: firefly-infra-delivery
description: Implement deployment, Docker, GitHub Actions, VS Code local-run, or infrastructure changes in the Firefly Signal repository. Use when Codex is editing infra files for this repo.
---

# Firefly Infrastructure Delivery

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- `infra/README.md`
- the relevant file in `infra/docs/`

Implementation rules:
- keep local dev infrastructure in `services/api/docker-compose.yml`
- keep production backend Dockerfiles in `infra/docker/`
- keep production runtime compose in `infra/deploy/`
- keep frontend deployment aligned with Cloudflare Pages
- prefer workflows that are explicit, secret-driven, and easy for a single maintainer to reason about

Before finishing:
- run the relevant local validation that exists
- summarize required secrets or host prerequisites
- update docs when deployment direction changes materially
