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
- `in-progress`
- `blocked`
- `ready-for-review`

Recommended meaning:
- `codex`: issue is suitable for Codex-assisted delivery
- `co-op`: issue will be worked collaboratively by you and Codex
- `in-progress`: Codex is actively working the issue
- `blocked`: Codex or the maintainer found an unresolved dependency
- `ready-for-review`: implementation is complete and awaiting your final review

Labels and status comments should work together.
For the current manual flow, Codex should always post issue status comments as the required visible status signal and keep labels aligned so the issue list also reflects the current state.

Recommended status comments:
- `in progress` when Codex picks up the issue and starts active work
- `blocked` when Codex cannot continue without clarification or an external dependency
- `ready for review` when implementation and validation are complete

Recommended label behavior:
- keep `codex` and `co-op` on active Codex issues
- add `in-progress` when Codex starts active work
- replace `in-progress` with `blocked` if progress stops
- replace `in-progress` or `blocked` with `ready-for-review` when implementation is complete
- keep `in-progress`, `blocked`, and `ready-for-review` mutually exclusive

## Suggested Issue Lifecycle

1. Create the issue with the `task` template.
2. Treat it as `co-op`.
3. Add `codex` only when you want Codex involved.
4. When Codex starts the issue, it adds an `in progress` status comment to the issue and applies `codex`, `co-op`, and `in-progress`.
5. Run the manual Codex command on the Mac server for `co-op` issues during the trial phase.
6. If Codex gets stuck, it adds a `blocked` status comment to the issue and swaps the status label to `blocked`.
7. Move the resulting PR into review.
8. When Codex finishes, it adds a `ready for review` status comment to the issue and swaps the status label to `ready-for-review`.

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
- `<type>(<scope>): <description> (#<issue-number>)`

PR bodies should include:
- `Closes #<issue-number>`

## PR Management Guidance

For Codex-created PRs:
- prefer draft PRs until checks and self-review are complete
- rebase the issue branch onto the latest target branch before opening the PR
- link the PR back to the issue
- keep one issue per PR
- treat the source issue number as the canonical work item ID
- do not use the PR number as a substitute for the issue number in commit messages, PR titles, PR bodies, or summaries
- record assumptions instead of hiding them
- leave final review and squash merge to the repository owner

## Initial Setup Recommendation

Before introducing OpenClaw, keep the management model simple:
- create issues manually
- keep the current workflow `co-op`
- trigger Codex manually
- review PRs manually

That gives the workflow a stable human-controlled baseline before any additional orchestration is introduced.
