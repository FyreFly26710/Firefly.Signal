# Cloudflare Pages

Firefly Signal web is intended to deploy to Cloudflare Pages.

## App
- source folder: `apps/web`
- build command: `npm run build`
- output folder: `dist`

## Notes
- client-side routing fallback is handled by `apps/web/public/_redirects`
- set the production API base URL in Cloudflare Pages environment variables as `VITE_API_BASE_URL`
- the GitHub workflow deploys with Wrangler using repository secrets
