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

`<type>: <description> (#<issue-number>)`

Examples:
- `feat: add postcode search page shell (#12)`
- `fix: correct search result empty state copy (#27)`
- `docs: add frontend environment variable notes (#34)`

Recommended PR types:
- `feat`
- `fix`
- `docs`
- `refactor`
- `test`
- `build`
- `chore`

Rules:
- use a lowercase type prefix
- keep the description concise and readable
- include the originating issue number at the end
- keep the PR tied to one issue

## Merge Strategy

The expected merge strategy is:
- rebase the issue branch onto the latest target branch before opening the PR
- let the repository owner perform final review
- use squash merge at the end

This keeps the review simple while preserving a clean project history on the main branch.
