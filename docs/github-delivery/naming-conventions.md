# GitHub Naming Conventions

This document defines the naming rules for GitHub issues, issue branches, and pull requests in Firefly Signal.

## Goals

The naming convention should make it easy to:
- connect an issue, branch, and PR at a glance
- keep automation simple later
- make Codex output predictable
- keep final review lightweight for the repository owner

## Issue Titles

Issue titles should describe the requested outcome clearly and directly.

Format:
- short descriptive phrase

Examples:
- `Add postcode search page shell`
- `Fix search result empty state`
- `Document frontend environment variables`

Rules:
- do not prefix issue titles with `feat`, `fix`, `docs`, or similar change types
- keep the title focused on one change
- prefer outcome language over implementation detail when possible

## Branch Names

Branches created from issues must use this format:

`issue-<number>-<descriptive-title>`

Examples:
- `issue-12-add-postcode-search-page-shell`
- `issue-27-fix-search-result-empty-state`
- `issue-34-document-frontend-environment-variables`

Rules:
- include the GitHub issue number
- convert the descriptive title to lowercase kebab-case
- remove filler words when they do not add clarity
- keep the branch focused on that one issue

## Pull Request Titles

PR titles must use this format:

`<type>(<scope>): <description> (#<issue-number>)`

Examples:
- `fix(auth): handle null token validation (#12)`
- `refactor(gateway): simplify routing configuration (#8)`
- `test(job-search): add demo endpoint tests (#15)`
- `chore(ci): update docker compose health checks (#3)`
- `agent(github-delivery): update github delivery skills (#10)`

Recommended PR types:
- `feat`
- `fix`
- `refactor`
- `test`
- `chore`
- `agent`

Rules:
- use a lowercase type prefix
- use a short lowercase scope that names the main area being changed
- keep the description concise and readable
- include the originating issue number at the end
- keep the PR tied to one issue
- treat the source issue number as the canonical work item ID
- do not use the pull request number as a substitute for the issue number

PR bodies must include:

`Closes #<issue-number>`

Example:

`Closes #12`

The PR title makes the issue reference easy to scan, but the PR body is what GitHub uses reliably for issue linking and auto-close behavior.

## Commit Messages

Commit messages should stay descriptive and should not use the pull request number as a substitute for the issue number.

Good examples:
- `docs: add development planning docs`
- `fix: handle null token validation`

If an issue number is included in a commit message, it should be the source issue number, not the PR number.

## Merge Strategy

The expected merge strategy is:
- rebase the issue branch onto the latest target branch before opening the PR
- let the repository owner perform final review
- use squash merge at the end

This keeps the review simple while preserving a clean project history on the main branch.
