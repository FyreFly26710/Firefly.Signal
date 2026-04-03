---
name: firefly-planning
description: Plan work in the Firefly Signal repository. Use when Codex needs to turn product ideas, architecture decisions, or issue outlines into concrete documentation, phased delivery plans, or implementation sequencing for this repo.
---

# Firefly Planning

Read `AGENTS.md`, `docs/plans.md`, and the relevant design document before proposing substantial work.

Keep planning output practical:
- prefer phased delivery over exhaustive speculation
- tie architecture choices back to the personal-use MVP
- preserve room for later mobile expansion without designing a mobile platform now

When planning backend work:
- keep the gateway plus job-search-service start point in mind
- avoid introducing RabbitMQ or Redis without a concrete flow that needs them
- prefer local Docker development and simple Mac mini deployment assumptions

When planning frontend work:
- keep the first release centered on a single search flow
- preserve React 18, TypeScript, Vite, Zustand, MUI, and Tailwind as the intended stack
- prioritize loading, error, and empty states

When planning implementation:
- propose small GitHub issues that fit into one branch and one reviewable PR
- call out assumptions and unresolved questions explicitly
- update docs if the recommendation materially changes repository direction
