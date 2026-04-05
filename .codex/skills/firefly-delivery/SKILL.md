---
name: firefly-delivery
description: Deliver direct implementation work in the Firefly Signal repository. Use when Codex is handling a coding task in this repo that is not primarily driven by a GitHub issue workflow.
---

# Firefly Delivery

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- the relevant design document for the touched area

Use this skill for direct repo work that does not start from a GitHub issue.
For GitHub issue-driven work, use `firefly-github-delivery` as the orchestration skill instead.

When implementing:
- keep changes focused on the requested task
- do not broaden scope into unrelated platform work
- follow existing repo patterns before introducing new abstractions
- keep the codebase understandable for a single maintainer

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
- summarize assumptions
- note any docs that should be updated with the change
- keep summaries readable for a human reviewer scanning quickly
