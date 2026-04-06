---
name: firefly-github-delivery
description: Orchestrate Firefly Signal GitHub issue-driven delivery. Use when Codex is reading a GitHub issue, planning the work, creating the issue branch, coordinating implementation, validating changes, and preparing the pull request.
---

# Firefly GitHub Delivery

Read these before acting:
- `AGENTS.md`
- `docs/plans.md`
- `docs/github-delivery/overview.md`
- `docs/github-delivery/naming-conventions.md`
- `docs/github-delivery/templates-and-management.md`
- `docs/github-delivery/manual-codex-flow.md`

Use this skill as the entry point for GitHub issue-driven work.
The GitHub issue is the source of truth for the requested change.
Prefer the connected GitHub app/tools first for issue, PR, and metadata access.
Use local `gh` commands as a helper or fallback when terminal GitHub workflow is useful and `gh` is installed and authenticated.

## Orchestration Rules

- Start by reading the issue carefully and restating its goal, scope, acceptance criteria, constraints, and useful context.
- Treat ambiguity in the issue as a blocker to clarify, not an invitation to invent scope.
- Read the relevant repo docs for the touched area before editing.
- Before substantive implementation work begins, update the source GitHub issue with a short status comment that says Codex has picked it up in `co-op` mode, names the working branch, and marks the issue status as `in progress`.
- Before substantive implementation work begins, ensure the source issue carries the workflow labels `codex` and `co-op`, add the `in-progress` label, and remove `blocked` or `ready-for-review` if they are present from an older state.
- Use `firefly-planning` when the issue needs refinement, sequencing, or issue breakdown before implementation.
- Use `firefly-frontend-delivery` for frontend implementation work.
- Use `firefly-backend-delivery` for backend implementation work.
- Keep the implementation scoped to the issue acceptance criteria and constraints.
- Keep one GitHub issue mapped to one focused branch and one reviewable PR.
- If the work becomes blocked, add a follow-up issue comment that marks the status as `blocked` and explains the blocker clearly.
- If the work becomes blocked, update labels so `blocked` is present and `in-progress` plus `ready-for-review` are removed.
- Before handing work back for review, add a follow-up issue comment that marks the status as `ready for review` and summarizes validation, assumptions, and any remaining risks.
- Before handing work back for review, update labels so `ready-for-review` is present and `in-progress` plus `blocked` are removed.

## Issue Status Rules

- Treat issue comments as the required source of visible status during the current manual `co-op` workflow.
- Treat issue comments as the required source of visible status during the current manual `co-op` workflow.
- Mirror that visible status with issue labels in GitHub so the issue list can also be filtered by workflow state.
- Required issue status transitions are:
  - `in progress` when Codex starts active work on the issue
  - `blocked` when Codex cannot continue without input or an external dependency
  - `ready for review` when implementation and validation are complete
- Required workflow label transitions are:
  - add `codex`, `co-op`, and `in-progress` when active work starts
  - swap to `blocked` if work cannot continue
  - swap to `ready-for-review` when the work is complete
- Keep `in-progress`, `blocked`, and `ready-for-review` mutually exclusive so one issue shows one current workflow state.
- Keep status comments short, specific, and human-readable so the issue timeline explains what happened without opening the branch or PR.

## Naming Rules

- GitHub issue titles should be short, descriptive, and focused on the outcome.
- Do not prefix issue titles with `feat`, `fix`, or other change types.
- Treat the source GitHub issue number as the canonical work item ID.
- Never use the pull request number as a substitute for the issue number in commit messages, PR titles, PR bodies, or summaries.
- Branch names must be `issue-<number>-<descriptive-title>`.
- Convert the descriptive title to lowercase kebab-case.
- PR titles must be `<type>(<scope>): <description> (#<issue-number>)`.
- PR bodies must include `Closes #<issue-number>`.
- Prefer PR types: `feat`, `fix`, `refactor`, `test`, `chore`, `agent`.

## Delivery Rules

- Create or switch to a branch named `issue-<number>-<descriptive-title>` before making issue-specific changes when branch work is part of the task.
- Implement the smallest change that satisfies the issue.
- Add or update tests when appropriate for the touched behavior.
- Run the relevant checks that exist.
- Keep the issue status comment aligned with the current state of the work as it changes.
- Keep the issue labels aligned with the same state transitions as the issue comments.
- Rebase the issue branch onto the latest target branch before opening the PR when the workflow expects a clean linear history.
- Leave final review and squash merge to the repository owner.

## Branch And PR Preparation

Before opening the PR:
- confirm the source issue number is still the canonical work item ID for the branch
- confirm the branch name matches the issue number and title
- confirm the PR title uses the required `type(scope): description (#issue)` format
- confirm the PR body includes `Closes #<issue-number>`
- confirm commit messages do not use the PR number in place of the issue number
- summarize validation, assumptions, and risks clearly for human review

Keep the user involved throughout the work.
Treat the current repo workflow as `co-op` by default, not autonomous `agent-only` delivery.
