# OpenClaw Future State

This document describes the intended later integration point for OpenClaw after the manual Codex flow is proven.

## Role Of OpenClaw

OpenClaw should act as a workflow trigger and coordinator, not as the place where repository delivery rules live.

That means:
- GitHub issue structure stays defined in this repository
- Codex prompting and repo guardrails stay defined in this repository
- OpenClaw decides when to run the agreed Codex command, based on your instruction

## Expected Future Flow

1. You create the issue in GitHub.
2. You decide whether the issue is `agent-only` or `co-op`.
3. For `agent-only`, you notify OpenClaw.
4. OpenClaw runs the Mac server command that starts the Codex issue workflow.
5. Codex implements the issue and creates a PR.
6. You perform the final review and merge decision.

## Important Guardrails

OpenClaw should not:
- auto-pick all issues by default
- bypass your decision about `agent-only` versus `co-op`
- merge without explicit human approval
- broaden scope beyond the issue

OpenClaw should:
- trigger the known Codex workflow
- pass the target issue identifier cleanly
- report success, blocked status, or failure clearly

## Recommended Integration Principle

Treat OpenClaw as an execution switch, not as a replacement for GitHub issue hygiene or repository delivery discipline.

That keeps the workflow understandable and makes it easier to debug:
- if issue quality is poor, fix the issue template or issue-writing habits
- if Codex performs poorly, improve the Codex prompt and repo docs
- if triggering is clumsy, improve OpenClaw orchestration
