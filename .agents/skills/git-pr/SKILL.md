---
name: git-pr
description: Prepare Firefly pull requests with the correct title, body, scope, and review handoff. Use when opening or finalizing a PR for issue-driven work.
origin: Firefly
---

# Git PR

Use this skill when preparing a pull request for review.

## When To Use
- Opening a new PR from an issue branch.
- Checking whether a branch is ready for PR creation.
- Rewriting a PR title or body to match repo rules.
- Preparing the review summary, assumptions, and risks.

## PR Contract
- Keep one issue mapped to one focused PR.
- Treat the source issue number as the canonical work item ID.
- Do not use the PR number as a substitute for the issue number in commit messages, titles, bodies, or summaries.

## Title Format
Use:

`<type>(<scope>): <description> (#<issue-number>)`

Preferred types:
- `feat`
- `fix`
- `refactor`
- `test`
- `chore`
- `agent`

## Body Requirements
- Include `Closes #<issue-number>`.
- Summarize what changed at a reviewable level.
- Call out validation performed.
- Call out important assumptions or risks when they matter.

## Workflow Rules
- Confirm the branch name matches the source issue.
- Rebase onto the latest target branch before opening the PR.
- Keep the PR in draft until the implementation and validation are ready for review.
- Hand back a concise summary that lets the reviewer understand the change quickly.

## Review Gate
Before opening the PR, confirm:
- scope still matches the issue
- title format is correct
- body includes the closing link
- validation summary is truthful
- assumptions and risks are not hidden
