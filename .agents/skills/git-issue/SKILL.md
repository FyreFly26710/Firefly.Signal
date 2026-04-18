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

Use the matching GitHub App bot for all issue updates:

- Codex uses `./scripts/with-github-app.sh codex-coder -- gh ...`
- Claude uses `./scripts/with-github-app.sh claudecode-coder -- gh ...`

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

## Round 1 Checklist — Issue Kickoff
Run all of these in one chat session before handing back to the developer.

1. Read this skill (`git-issue`).
2. Fetch the issue with the matching bot:
   - Codex: `./scripts/with-github-app.sh codex-coder -- gh issue view <number>`
   - Claude: `./scripts/with-github-app.sh claudecode-coder -- gh issue view <number>`
3. Add the `in-progress` label with the matching bot:
   - Codex: `./scripts/with-github-app.sh codex-coder -- gh issue edit <number> --add-label "in-progress"`
   - Claude: `./scripts/with-github-app.sh claudecode-coder -- gh issue edit <number> --add-label "in-progress"`
4. Derive branch name (`issue-<number>-<title>`). Create and push if it does not exist:
   ```
   git checkout -b issue-<number>-<title>
   git push -u origin issue-<number>-<title>
   ```
   If the branch already exists, check it out and push any pending commits.
5. Create a git worktree so parallel agents can work on the same server without collision:
   ```
   mkdir -p worktrees
   git worktree add worktrees/issue-<number>-<title> issue-<number>-<title>
   ```
   Worktrees live at `worktrees/<branch-name>` inside the repo.
6. Read `AGENTS.md` and relevant source files to understand existing patterns.
7. Enrich the issue body — **append below any existing description, never overwrite it**. Fill in `Goal`, `Scope`, `Acceptance Criteria`, and `Constraints` based on what you learned from the repo.
8. Post a comment with the branch name and worktree path:
   - Codex:
     ```
     ./scripts/with-github-app.sh codex-coder -- gh issue comment <number> --body "Branch: issue-<number>-<title>\nWorktree: worktrees/issue-<number>-<title>"
     ```
   - Claude:
     ```
     ./scripts/with-github-app.sh claudecode-coder -- gh issue comment <number> --body "Branch: issue-<number>-<title>\nWorktree: worktrees/issue-<number>-<title>"
     ```
9. Post a second comment with a numbered implementation plan:
   - Codex:
     ```
     ./scripts/with-github-app.sh codex-coder -- gh issue comment <number> --body "Implementation plan:\n1. ...\n2. ..."
     ```
   - Claude:
     ```
     ./scripts/with-github-app.sh claudecode-coder -- gh issue comment <number> --body "Implementation plan:\n1. ...\n2. ..."
     ```

## Further Rounds — Implementation
- Work inside the worktree: `worktrees/<branch-name>`.
- Commit and push before ending every chat session so the developer can review progress.
