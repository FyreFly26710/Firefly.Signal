#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <agent-slug> [owner/repo]" >&2
  exit 1
fi

agent_slug="$1"
repo_slug="${2:-}"
repo_root="$(git rev-parse --show-toplevel)"
app_dir="$repo_root/sources/github-apps/$agent_slug"
app_info_path="$app_dir/app-info.json"

if [[ ! -f "$app_info_path" ]]; then
  echo "Missing app info file: $app_info_path" >&2
  exit 1
fi

app_id="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["app_id"])' "$app_info_path")"
private_key_path="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["private_key_path"])' "$app_info_path")"

if [[ "$private_key_path" != /* ]]; then
  private_key_path="$app_dir/$private_key_path"
fi

if [[ ! -f "$private_key_path" ]]; then
  echo "Missing private key file: $private_key_path" >&2
  exit 1
fi

if [[ -z "$repo_slug" ]]; then
  origin_url="$(git remote get-url origin)"
  repo_slug="$(python3 - "$origin_url" <<'PY'
import re
import sys

url = sys.argv[1]
match = re.search(r"github\.com[:/](.+?)(?:\.git)?$", url)
if not match:
    raise SystemExit(f"Could not parse GitHub repo from remote URL: {url}")
print(match.group(1))
PY
)"
fi

api_url="https://api.github.com"
now="$(date +%s)"
iat="$((now - 60))"
exp="$((now + 540))"

base64url() {
  openssl base64 -A | tr '+/' '-_' | tr -d '='
}

header_b64="$(printf '{"alg":"RS256","typ":"JWT"}' | base64url)"
payload_b64="$(printf '{"iat":%s,"exp":%s,"iss":"%s"}' "$iat" "$exp" "$app_id" | base64url)"
unsigned_token="$header_b64.$payload_b64"
signature_b64="$(printf '%s' "$unsigned_token" | openssl dgst -binary -sha256 -sign "$private_key_path" | base64url)"
jwt="$unsigned_token.$signature_b64"

installation_response="$(
  curl -fsSL \
    -H "Accept: application/vnd.github+json" \
    -H "Authorization: Bearer $jwt" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    "$api_url/repos/$repo_slug/installation"
)"

installation_id="$(printf '%s' "$installation_response" | python3 -c 'import json,sys; print(json.load(sys.stdin)["id"])')"

token_response="$(
  curl -fsSL \
    -X POST \
    -H "Accept: application/vnd.github+json" \
    -H "Authorization: Bearer $jwt" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    "$api_url/app/installations/$installation_id/access_tokens"
)"

printf '%s\n' "$(printf '%s' "$token_response" | python3 -c 'import json,sys; print(json.load(sys.stdin)["token"])')"
