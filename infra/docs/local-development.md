# Local Development

This document defines the intended local development model.

## Backend
- run PostgreSQL, pgweb, Redis, RedisInsight, and RabbitMQ through `services/api/docker-compose.yml`
- run APIs directly from VS Code or `dotnet run`
- keep the dev loop focused on source edits, not local image builds

## Frontend
- run Vite directly from `apps/web`
- point the frontend to the local gateway with `VITE_API_BASE_URL`

## VS Code

The repo should support:
- bringing up backend infra quickly
- starting all backend APIs together
- starting the frontend app
- starting a full-stack local session from launch profiles
