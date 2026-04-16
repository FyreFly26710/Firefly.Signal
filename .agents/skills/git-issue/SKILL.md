---
name: git-issue
description: Shape and manage Firefly GitHub issues so they stay reviewable, explicit, and safe to implement. Use for issue refinement, issue-driven execution, and status handling.
origin: Firefly
---

# Git Issue

Use this skill when the work is driven by a GitHub issue or when a new issue needs to be prepared for implementation.

## When To Use
- Fetching and refining an existing issue.
- Turning a vague request into a concrete issue body.
- Updating issue status during active work.
- Checking whether an issue is ready to implement.

## Issue Contract
- The GitHub issue is the source of truth for the requested change.
- Keep the title short and descriptive.
- Do not prefix issue titles with `feat`, `fix`, `docs`, or similar change types.
- Keep one issue scoped to one focused branch and one reviewable PR whenever possible.

## Recommended Issue Body
- `Description`
- `Goal`
- `Scope`
- `Acceptance Criteria`
- `Constraints`
- `Context`

Not every field has to be long, but the issue should be concrete enough to implement without guessing hidden scope.

## Refinement Workflow
1. Restate the issue in plain language.
2. Surface ambiguity instead of inventing scope.
3. Propose concrete `Goal`, `Scope`, and `Acceptance Criteria`.
4. Ask for confirmation before materially rewriting the issue body.
5. Once aligned, update the issue so the final task definition lives there.

## Status Handling
Use visible issue comments and aligned labels during issue-driven work.

- `in progress`
  Active implementation has started.
- `blocked`
  Work cannot continue without a dependency or clarification.
- `ready for review`
  Implementation and validation are complete.

Keep `in-progress`, `blocked`, and `ready-for-review` mutually exclusive.

## Naming Rule
Branches created from issues should use:

`issue-<number>-<descriptive-title>`

Use lowercase kebab-case after the number.
