# GitHub Agent Bots

This repo can let local coding agents act on GitHub as GitHub App bots instead of as a personal user.

The current pattern is:

- one local app folder per agent under `sources/github-apps/<agent-slug>/`
- one `app-info.json` file per agent with the app metadata and relative private key filename
- helper scripts in `scripts/` that mint a short-lived installation token for the current repo

## Local App Folder Layout

Each agent keeps local-only app files under:

```text
sources/github-apps/<agent-slug>/
  app-info.json
  *.private-key.pem
```

These files stay local because `sources/` is gitignored.

Example `app-info.json`:

```json
{
  "name": "FF-Codex-Coder",
  "app_id": "3421289",
  "client_id": "Iv23liQi957OdPDubrvR",
  "private_key_path": "ff-codex-coder.2026-04-18.private-key.pem",
  "created_at": "2026-04-18"
}
```

The `private_key_path` value should be relative to the agent's own folder so the repo can move without breaking the setup.

## Bot Commands

Mint a token for the current repo:

```bash
./scripts/github-app-token.sh codex-coder
```

Run any `gh` command as the bot:

```bash
./scripts/with-github-app.sh codex-coder -- gh issue view 114
./scripts/with-github-app.sh codex-coder -- gh issue comment 114 --body "Codex bot test"
./scripts/with-github-app.sh codex-coder -- gh issue edit 114 --add-label "in-progress"
./scripts/with-github-app.sh codex-coder -- gh pr comment 115 --body "Codex bot update"
```

Target another repo explicitly:

```bash
./scripts/with-github-app.sh codex-coder FyreFly26710/Firefly.Signal -- gh issue view 114 --repo FyreFly26710/Firefly.Signal
```

## Current Agents

- `codex-coder`
- `claudecode-coder`
- `reviewer` later if needed

Keep the local folder slug stable because scripts use it directly.

## Notes

- GitHub App installation tokens are short-lived.
- The helper script scopes the token to the current repository by resolving the repo's GitHub remote.
- For API compatibility, the scripts use the GitHub REST API version `2022-11-28`.
- This local setup does not require GitHub Actions secrets; the app metadata and PEM stay in `sources/github-apps/` on the machine that runs the agents.
