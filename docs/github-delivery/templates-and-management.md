# Issue And PR Templates And Management

This document defines how GitHub issues and pull requests should be structured for Codex-assisted delivery.

Naming conventions for issues, branches, and PR titles are defined in:
- `naming-conventions.md`

## Issue Template Direction

Use a dedicated issue template for implementation work intended for Codex.

Current repository file:
- `.github/ISSUE_TEMPLATE/task.yml`

The issue template should keep each task reviewable and unambiguous.
The current fields should stay intentionally simple:
- `Goal`
- `Scope`
- `Acceptance Criteria`
- `Constraints`
- `Context` (optional)

Issue titles should follow the naming guidance in `naming-conventions.md`:
- short descriptive phrase
- no `feat` or `fix` prefix in the issue title

## Issue Writing Guidance

Each Codex issue should:
- describe one concrete outcome
- define what done looks like
- call out what is in scope and out of scope through scope and constraints
- exclude unrelated cleanup

Good issues are:
- small enough for one branch and one PR
- explicit about constraints
- written so a coding agent does not have to infer product intent from scratch

Avoid issues that:
- mix frontend, backend, infra, and product changes without a clear single goal
- ask for broad refactors without acceptance criteria
- leave testing expectations unstated

## Suggested Labels

Use a small, stable label set so the workflow stays easy to manage.

Recommended labels:
- `task`
- `codex`
- `co-op`
- `blocked`
- `ready-for-review`

Recommended meaning:
- `codex`: issue is suitable for Codex-assisted delivery
- `co-op`: issue will be worked collaboratively by you and Codex
- `blocked`: Codex or the maintainer found an unresolved dependency
- `ready-for-review`: implementation is complete and awaiting your final review

You do not need all of these on day one.
For the current manual flow, `task`, `codex`, and `co-op` are enough.

## Suggested Issue Lifecycle

1. Create the issue with the `task` template.
2. Treat it as `co-op`.
3. Add `codex` only when you want Codex involved.
4. Run the manual Codex command on the Mac server for `co-op` issues during the trial phase.
5. Move the resulting PR into review.
6. Use `ready-for-review` when Codex has finished and the final decision is yours.

## Pull Request Template Direction

Current repository file:
- `.github/pull_request_template.md`

The current PR template is aligned with the intended workflow:
- `Summary`
- `Why`
- `Validation`
- `Assumptions`
- `Risks`

This is a good fit for Codex-created PRs because it keeps the reviewer focused on:
- what changed
- why it changed
- what was checked
- where the reviewer should look carefully

PR titles should follow the naming guidance in `naming-conventions.md`:
- `<type>: <description> (#<issue-number>)`

PR bodies should include:
- `Closes #<issue-number>`

## PR Management Guidance

For Codex-created PRs:
- prefer draft PRs until checks and self-review are complete
- rebase the issue branch onto the latest target branch before opening the PR
- link the PR back to the issue
- keep one issue per PR
- record assumptions instead of hiding them
- leave final review and squash merge to the repository owner

## Initial Setup Recommendation

Before introducing OpenClaw, keep the management model simple:
- create issues manually
- keep the current workflow `co-op`
- trigger Codex manually
- review PRs manually

That gives the workflow a stable human-controlled baseline before any additional orchestration is introduced.
