# GitHub Delivery Overview

This folder documents the intended GitHub issue to branch to PR flow for Firefly Signal.

The initial goal is not full automation.
The first goal is to prove that a small, focused issue can be created in GitHub, picked up manually on the Mac server, implemented by Codex, validated, and returned as a reviewable pull request.

## Delivery Stages

### Stage 1: Manual Codex Execution
- You create the issue yourself in GitHub.
- You use the issue as the source of truth and work with Codex in `co-op` mode.
- You manually run a command on the Mac server to let Codex pull the issue context and begin refinement.
- Codex updates the issue timeline with visible status comments such as `in progress`, `blocked`, and `ready for review` while the work moves through the manual flow.
- Codex also keeps the matching workflow labels in sync so the issue list reflects the same state.
- Codex reads the relevant docs, reviews the issue, proposes a refined `Goal`, `Scope`, and `Acceptance Criteria`, and waits for your confirmation before coding.
- After you align on the task shape, Codex updates the issue body so the refined issue remains the source of truth.
- Codex then creates or continues the focused branch, implements the change, runs the relevant checks, and prepares a PR.
- You perform the final human review before merge.

### Stage 2: OpenClaw-Assisted Triggering
- You still create and triage the issue yourself.
- When the co-op workflow is stable, you can notify OpenClaw to start the same issue-driven flow.
- OpenClaw runs the agreed command on the Mac server to start the Codex workflow.
- Codex still keeps the issue scoped to one reviewable PR.

### Stage 3: Optional Stronger Automation
- GitHub events can later notify OpenClaw or another orchestrator automatically.
- Automatic pickup should remain label- or command-gated so you stay in control of which issues Codex handles alone.
- Full automation should only be introduced after the manual path is reliable.

## Recommended Operating Model

- Keep issue creation human-led.
- Keep the current workflow `co-op`.
- Keep issue refinement collaborative before implementation starts.
- Keep Codex branches focused on one issue.
- Keep pull requests in draft until Codex has finished implementation and validation.
- Keep merge approval human-led.

## Why This Order

This order reduces risk:
- the issue format becomes stable before automation is added
- the Codex prompt and branch workflow can be tested without also debugging webhooks
- OpenClaw can be added later as an orchestration layer instead of being a hard dependency on day one

`agent-only` can remain a future option, but it is not part of the active operating model yet.

## Suggested Read Order

1. `naming-conventions.md`
2. `templates-and-management.md`
3. `manual-codex-flow.md`
4. `openclaw-future-state.md`
