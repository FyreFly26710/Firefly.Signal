# CLAUDE.md

This repository uses a shared instruction model for both Claude and Codex.

## Start Here
- Read [AGENTS.md](/Users/admin/Repo/Firefly.Signal/AGENTS.md).
- Use the shared skills in `/Users/admin/Repo/Firefly.Signal/.agents/skills/`.
- Treat `docs/` as product, planning, and project-explanation material rather than implementation-pattern guidance.

## Commands

### Frontend

```bash
cd apps/web
npm run dev
npm run build
npm run lint
npm test
```

### Backend

```bash
dotnet restore services/api/Firefly.Signal.Api.slnx
dotnet build services/api/Firefly.Signal.Api.slnx
```

### Local Infrastructure

```bash
cd services/api
docker-compose up
```

## Shared Skills
- `documentation-lookup`
- `frontend-design`
- `frontend-patterns`
- `backend-patterns`
- `git-issue`
- `git-pr`

Use the shared skill body as the source of truth. The repo should not maintain separate Claude-only architecture rules.
