---
name: firefly-delivery
description: Deliver implementation work in the Firefly Signal repository. Use when Codex is handling a concrete GitHub issue or coding task in this repo and needs to create focused changes, validate them, and prepare reviewable pull request output.
---

# Firefly Delivery

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- the relevant design document for the touched area

When implementing:
- keep changes focused on the issue at hand
- do not broaden scope into unrelated platform work
- follow existing repo patterns before introducing new abstractions
- keep the codebase understandable for a single maintainer
- use a branch named `issue-<number>-<descriptive-title>` when the work comes from a GitHub issue

For frontend tasks:
- preserve the intended stack: React 18, TypeScript, Vite, Zustand, MUI, Tailwind
- keep API calls behind a small client boundary
- treat loading, empty, and error states as required behavior

For backend tasks:
- preserve the gateway and service boundary
- prefer explicit contracts and vertical slices
- avoid adding distributed complexity unless the issue needs it

Before finishing:
- run the relevant checks that exist
- rebase the branch onto the latest target branch before opening a PR when that fits the workflow
- summarize assumptions
- note any docs that should be updated with the change
- keep PR summaries readable for a human reviewer scanning quickly
- use a PR title in the form `<type>: <description> (#<issue-number>)`
- add `Closes #<issue-number>` to the PR body
