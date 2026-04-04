# GitHub Delivery Overview

This folder documents the intended GitHub issue to branch to PR flow for Firefly Signal.

The initial goal is not full automation.
The first goal is to prove that a small, focused issue can be created in GitHub, picked up manually on the Mac server, implemented by Codex, validated, and returned as a reviewable pull request.

## Delivery Stages

### Stage 1: Manual Codex Execution
- You create the issue yourself in GitHub.
- You decide whether the issue is small enough for agent-only delivery or whether it needs collaboration.
- For agent-only issues, you manually run a command on the Mac server to let Codex pull the issue context and begin work.
- Codex creates a focused branch, implements the change, runs the relevant checks, and prepares a PR.
- You perform the final human review before merge.

### Stage 2: OpenClaw-Assisted Triggering
- You still create and triage the issue yourself.
- When you decide the issue is ready for agent-only delivery, you notify OpenClaw.
- OpenClaw runs the agreed command on the Mac server to start the Codex workflow.
- Codex still keeps the issue scoped to one reviewable PR.

### Stage 3: Optional Stronger Automation
- GitHub events can later notify OpenClaw or another orchestrator automatically.
- Automatic pickup should remain label- or command-gated so you stay in control of which issues Codex handles alone.
- Full automation should only be introduced after the manual path is reliable.

## Recommended Operating Model

- Keep issue creation human-led.
- Keep the decision between `agent-only` and `co-op` human-led.
- Keep Codex branches focused on one issue.
- Keep pull requests in draft until Codex has finished implementation and validation.
- Keep merge approval human-led.

## Why This Order

This order reduces risk:
- the issue format becomes stable before automation is added
- the Codex prompt and branch workflow can be tested without also debugging webhooks
- OpenClaw can be added later as an orchestration layer instead of being a hard dependency on day one

## Suggested Read Order

1. `naming-conventions.md`
2. `templates-and-management.md`
3. `manual-codex-flow.md`
4. `openclaw-future-state.md`
