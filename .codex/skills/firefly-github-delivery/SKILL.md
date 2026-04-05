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
- Use `firefly-planning` when the issue needs refinement, sequencing, or issue breakdown before implementation.
- Use `firefly-frontend-delivery` for frontend implementation work.
- Use `firefly-backend-delivery` for backend implementation work.
- Keep the implementation scoped to the issue acceptance criteria and constraints.
- Keep one GitHub issue mapped to one focused branch and one reviewable PR.

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
