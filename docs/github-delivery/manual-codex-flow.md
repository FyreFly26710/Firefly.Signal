# Manual Codex Flow

This document describes the first delivery loop to prove before adding OpenClaw automation.

## Goal

Verify that Codex can take a small GitHub issue, work on it autonomously from the Mac server, and return a pull request that is ready for your final review.

## Preconditions

Before running the flow:
- the repository is cloned on the Mac server
- `codex` is installed and authenticated on the Mac server
- Git is configured so branches can be created and pushed
- the issue is small, focused, and written with the `task` issue template

## Current Working Mode

After creating an issue, treat it as `co-op`.

- `co-op`
  Use this when you and Codex are working the issue together, using the issue as the source of truth during implementation.

`agent-only` is not part of the current workflow.
It can be revisited later after the co-op flow is stable.

## Recommended Manual Flow

1. Create a GitHub issue using the `task` template.
2. Label it as `codex` and `co-op` if you want Codex involved in the issue-driven workflow.
3. On the Mac server, start from a clean checkout of the repository.
4. Run a Codex command that:
   - reads the issue details
   - posts an issue status update when work begins
   - creates a focused branch
   - implements only the requested scope
   - runs relevant checks
   - posts a blocked status comment if the work cannot continue
   - rebases onto the latest target branch before PR creation
   - posts a ready-for-review status comment when the work is complete
   - prepares a reviewable pull request
5. Review the branch and PR output yourself before merge.

## Command Design Guidance

The exact trigger command can evolve, but it should always preserve the same contract:
- point Codex at one issue only
- require it to treat the issue as the source of truth
- require it to read `AGENTS.md` and the relevant docs first
- require it to post visible issue status comments as work progresses
- keep scope limited to the issue
- require branch naming and PR title formatting to match repo conventions
- require the PR body to include `Closes #<issue-number>`
- require validation before finishing
- require a rebase before PR creation
- require a short PR-quality summary of assumptions and risks

The eventual OpenClaw command should call the same underlying workflow.

## Suggested Prompt Contract

When you build the manual command, the prompt should tell Codex to:
- fetch and summarize the target issue
- restate the issue goal, scope, acceptance criteria, constraints, and context
- create a branch named for the issue
- mark the issue `in progress` with a short status comment when work starts
- implement the smallest change that satisfies the issue
- add or update tests when appropriate
- run relevant checks
- mark the issue `blocked` with a short status comment if work cannot continue
- rebase onto the latest target branch before opening the PR
- mark the issue `ready for review` with a short status comment when implementation and validation are complete
- use the repo PR title convention
- add `Closes #<issue-number>` to the PR body
- prepare the change for human review
- stop and report clearly if blocked

## Success Criteria For The Trial

The first trial is successful if:
- Codex stays within the issue scope
- Codex produces a focused branch
- Codex runs the relevant checks that exist
- the PR summary is readable and useful
- you can perform a final review without reconstructing the whole task from scratch

## What Not To Automate Yet

Do not introduce these until the manual path is proven:
- automatic issue pickup from GitHub events
- automatic merge
- multi-issue queue processing
- automatic handling of ambiguous or design-heavy issues

## Next Step After The Manual Trial

Once the manual command works reliably, OpenClaw can be added as a thin orchestrator:
- OpenClaw runs the same command on the Mac server
- the human review gate remains unchanged
- any future `agent-only` mode should be considered separately from the current co-op flow
