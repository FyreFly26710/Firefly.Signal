# Manual Codex Flow

This document describes the first delivery loop to prove before adding OpenClaw automation.

## Goal

Verify that Codex can take a small GitHub issue, work on it autonomously from the Mac server, and return a pull request that is ready for your final review.

## Preconditions

Before running the flow:
- the repository is cloned on the Mac server
- `codex` is installed and authenticated on the Mac server
- Git is configured so branches can be created and pushed
- the issue is small, focused, and written with the Codex task template

## Human Decision Point

After creating an issue, decide which mode it belongs to:

- `agent-only`
  Use this when the issue is narrow, low-risk, and has clear acceptance criteria.

- `co-op`
  Use this when the issue needs active design discussion, hidden context, or tradeoff decisions during implementation.

Only `agent-only` issues should be used for the first autonomous trial.

## Recommended Manual Flow

1. Create a GitHub issue using the Codex task template.
2. Label it as `codex` and `agent-only` if you want Codex to handle it alone.
3. On the Mac server, start from a clean checkout of the repository.
4. Run a Codex command that:
   - reads the issue details
   - creates a focused branch
   - implements only the requested scope
   - runs relevant checks
   - prepares a reviewable pull request
5. Review the branch and PR output yourself before merge.

## Command Design Guidance

The exact trigger command can evolve, but it should always preserve the same contract:
- point Codex at one issue only
- require it to read `AGENTS.md` and the relevant docs first
- keep scope limited to the issue
- require validation before finishing
- require a short PR-quality summary of assumptions and risks

The eventual OpenClaw command should call the same underlying workflow.

## Suggested Prompt Contract

When you build the manual command, the prompt should tell Codex to:
- fetch and summarize the target issue
- create a branch named for the issue
- implement the smallest change that satisfies the issue
- add or update tests when appropriate
- run relevant checks
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
- you still decide which issues are `agent-only`
- OpenClaw runs the same command on the Mac server
- the human review gate remains unchanged
