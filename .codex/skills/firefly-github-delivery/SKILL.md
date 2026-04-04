---
name: firefly-github-delivery
description: Apply Firefly Signal GitHub issue, branch, and pull request delivery conventions. Use when Codex is turning a GitHub issue into a branch, preparing a pull request, or documenting repo delivery rules for issue-to-PR work.
---

# Firefly GitHub Delivery

Read these before acting:
- `AGENTS.md`
- `docs/plans.md`
- `docs/github-delivery/overview.md`
- `docs/github-delivery/naming-conventions.md`

Use this skill when the work involves GitHub issue-driven delivery or when Codex is preparing a branch or PR for review.

## Naming Rules

- GitHub issue titles should be short, descriptive, and focused on the outcome.
- Do not prefix issue titles with `feat`, `fix`, or other change types.
- Branch names must be `issue-<number>-<descriptive-title>`.
- Convert the descriptive title to lowercase kebab-case.
- PR titles must be `<type>: <description> (#<issue-number>)`.
- PR bodies must include `Closes #<issue-number>`.
- Prefer conventional PR types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `chore`.

## Delivery Rules

- One GitHub issue should map to one focused branch and one reviewable PR.
- Read the issue and relevant repo docs before editing.
- Keep the implementation scoped to the issue acceptance criteria.
- Run the relevant checks that exist.
- Rebase the issue branch onto the latest target branch before opening the PR when the workflow expects a clean linear history.
- Leave final review and squash merge to the repository owner.

## Branch And PR Preparation

Before opening the PR:
- confirm the branch name matches the issue number and title
- confirm the PR title uses the required type and issue number
- confirm the PR body includes `Closes #<issue-number>`
- summarize validation, assumptions, and risks clearly for human review

If issue details are ambiguous, stop and surface the ambiguity instead of inventing scope.
