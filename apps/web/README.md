# Firefly Signal Web

This app is the web-first frontend for Firefly Signal.

## Stack
- React 18
- TypeScript
- Vite
- MUI
- Tailwind CSS

## Local Development
1. Start backend infra with `docker compose --env-file .env.example up -d` from `services/api`.
2. Run backend APIs from VS Code launch profiles or `dotnet run`.
3. Run the web app from `apps/web` with `npm install` and `npm run dev`.

The frontend talks to the gateway by default through `VITE_API_BASE_URL`, which is set in `.env.example`.

## Build
- `npm run lint`
- `npm run test`
- `npm run build`

## Deployment

The intended production target is Cloudflare Pages.
Deployment workflows and production deployment docs live under `infra/`.
