# Cloudflare Tunnel On Mac Mini

This guide is for setting up the backend tunnel on the Mac mini.

## Goal

Expose the Firefly Signal backend gateway running on the Mac mini through Cloudflare Tunnel without opening inbound ports on the home network.

The expected target is:
- local backend gateway on the Mac mini, usually `http://localhost:8080`
- public hostname managed in Cloudflare
- Cloudflare Tunnel forwarding traffic to the local gateway container or process

## Prerequisites

- Cloudflare account with the target domain already in Cloudflare
- Mac mini with Docker and Docker Compose installed
- backend deployment directory prepared
- gateway container or process reachable locally on the Mac mini
- ability to install `cloudflared`

## 1. Install `cloudflared`

On macOS, the usual approach is Homebrew:

```bash
brew install cloudflared
```

Verify:

```bash
cloudflared --version
```

## 2. Authenticate With Cloudflare

Run:

```bash
cloudflared tunnel login
```

This opens a browser flow and stores the certificate needed to manage tunnels for the account.

## 3. Create A Named Tunnel

Create a dedicated tunnel for Firefly Signal:

```bash
cloudflared tunnel create firefly-signal
```

This returns a tunnel ID and creates credentials under the local Cloudflare config directory.

Keep the tunnel ID and generated credentials file path.

## 4. Create DNS Routing

Create the public hostname that should route to the tunnel:

```bash
cloudflared tunnel route dns firefly-signal api.your-domain.com
```

Recommended first target:
- `api.your-domain.com` -> Firefly Signal gateway

You can add more hostnames later if needed, but keep the first production path simple.

## 5. Create The Tunnel Config File

Create `~/.cloudflared/config.yml` with a structure like:

```yaml
tunnel: <TUNNEL_ID>
credentials-file: /Users/<your-user>/.cloudflared/<TUNNEL_ID>.json

ingress:
  - hostname: api.your-domain.com
    service: http://localhost:8080
  - service: http_status:404
```

Notes:
- point the service to the local gateway endpoint
- if the gateway is published on a different host port, update the URL accordingly
- keep the catch-all `404` rule at the end

## 6. Test The Tunnel Locally

Run:

```bash
cloudflared tunnel run firefly-signal
```

Then verify:
- the tunnel connects successfully
- the public hostname resolves through Cloudflare
- requests reach the local gateway

## 7. Install As A macOS Service

Once the tunnel works, install it as a persistent service:

```bash
sudo cloudflared service install
```

Depending on the local setup, you may instead prefer a user-level launch agent managed manually.
The separate Mac mini setup session can choose the cleaner option for that machine.

## 8. Operational Checks

After installation, confirm:
- the tunnel service starts on boot
- the backend gateway is reachable locally
- the public hostname returns gateway responses
- Cloudflare Tunnel status looks healthy in the Zero Trust dashboard

## Recommended Firefly Signal Rule Set

- tunnel only the gateway, not each internal API separately
- keep internal API traffic on the local Docker network
- keep the public DNS entry stable even if backend containers are redeployed
- treat the tunnel config as host-specific operational configuration, not a repository secret file

## Suggested Next Steps On The Mac Mini

1. install `cloudflared`
2. create the named tunnel
3. configure the public hostname
4. point ingress to the gateway service
5. verify live connectivity
6. install the tunnel as a persistent service
