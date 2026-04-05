# Development Roadmap

This roadmap translates the repository's current state and intended direction into a practical implementation sequence.
It is intentionally biased toward getting to a usable MVP first, then hardening and expanding the platform without prematurely over-engineering it.

## Planning Assumptions

- The web app is the primary client for the first release.
- The current codebase already contains an initial frontend search flow and backend service shells.
- Authentication should exist early, even if the first login flow is intentionally simple and uses a hardcoded password.
- The UI should be shaped and reviewed early using mock data before every backend capability is fully implemented.
- Job search remains the most important MVP feature after the first auth and layout foundations are in place.
- The repository owner remains the primary user and reviewer.

## Current State Summary

The repo already has:
- a web app with postcode and keyword inputs, results UI, and tests
- a gateway API and downstream identity, job search, and AI APIs
- database contexts and initial migrations for identity and job search
- RabbitMQ and event bus infrastructure in place for future async use
- local development and deployment assets under `services/api` and `infra`

The roadmap should therefore focus on turning that foundation into a stable MVP path rather than restarting the architecture from scratch.

## Recommended Delivery Sequence

### Phase 1: Establish Simple Auth For Early Protected Flows

Goal:
Create a minimal account and password JWT login flow that is good enough to unlock protected app flows during MVP development.

Recommended issue themes:
- expose a simple login endpoint through the identity API
- support a hardcoded password path for the initial personal-use login
- issue and validate JWTs through the existing gateway and identity boundary
- add the minimum frontend login experience needed to exercise authenticated flows
- document what is intentionally temporary in the MVP auth design

Why now:
- it creates a usable auth foundation without waiting for full identity design
- it keeps the MVP moving while preserving a path to better auth later

### Phase 2: Shape The Web UI With Mock Data

Goal:
Establish the main application layout and user flow with mock data so the product shape is visible before every backend feature is complete.

Recommended issue themes:
- define the main authenticated app layout
- build the initial pages or sections using mock job and profile-oriented data where useful
- refine navigation, empty states, and loading placeholders
- keep UI contracts aligned with likely backend response shapes without over-committing
- identify which mocked areas can later be replaced by real API integration with minimal rewrite

Why next:
- it makes the MVP tangible early
- it reduces UI uncertainty before deeper backend integration work

### Phase 3: Implement Real Job Search

Goal:
Turn the current search slice into a genuinely useful product capability.

Recommended issue themes:
- confirm and document the end-to-end search contract
- improve search validation rules for postcode and keyword input
- integrate the first public job source behind the job search service
- normalize and present real search results clearly in the web app
- add or refine tests around request handling, search results, and core UX states

Why here:
- job search is the core MVP value
- the earlier auth and layout work should make this phase easier to integrate and review

### Phase 4: Improve Authentication And Account Management

Goal:
Replace the intentionally simple MVP auth path with a more durable identity model.

Recommended issue themes:
- add Gmail Google OAuth sign-in
- support user registration and account management flows where needed
- clarify which identity features are truly required for the personal-use product
- keep JWT issuance and validation centralized while reducing hardcoded assumptions

Guardrail:
- do not turn identity into a large platform before the surrounding product flow justifies it

### Phase 5: Expand The AI API

Goal:
Move the AI API from placeholder shape toward useful but bounded product support.

Recommended issue themes:
- define the first non-trivial AI endpoints
- keep AI outputs clearly scoped to supporting the job discovery workflow
- add simple request and response contracts that can evolve safely
- avoid background orchestration unless a concrete workflow needs it

Guardrail:
- AI support should improve an existing flow, not become a disconnected feature branch

### Phase 6: Add Advanced Job Search And AI-Assisted Features

Goal:
Layer in richer search and AI-assisted product behavior after the MVP loop works.

Recommended issue themes:
- richer filtering and result refinement
- saved or repeatable search behavior if it supports real usage
- AI-assisted ranking, categorization, or job insight generation
- resume-aware workflows only when the core job discovery path remains understandable

Guardrail:
- advanced features should stay subordinate to a clear, dependable core experience

### Phase 7: Post-MVP Refactor And Architectural Hardening

Goal:
Use the completed MVP and real development experience to guide any deeper architectural refinement.

Recommended issue themes:
- identify pain points that the MVP exposed
- improve testing strategy and TDD discipline where it adds clarity
- refactor service boundaries, layering, or abstractions only where current code proves the need
- simplify or harden areas that became difficult to change during MVP delivery

Guardrail:
- do not refactor into a more complex architecture only because it sounds cleaner on paper
- use real friction from MVP work as the justification for structural change

## How To Turn This Roadmap Into Issues

Each issue should:
- target one concrete user or maintainer outcome
- touch one main concern where possible
- fit into one focused branch
- include acceptance criteria that can be validated locally

Good issue examples from this roadmap:
- add simple JWT login with hardcoded password for MVP access
- build the authenticated app shell using mock data
- integrate the first public job provider into the job search service
- add Gmail OAuth after MVP login flow proves the surrounding UX

## Roadmap Maintenance Rule

Update this roadmap when:
- the recommended sequencing changes materially
- a major phase has been completed
- a new constraint changes what should happen next

Do not update it for every small implementation detail.
Use `todo.md` for the more changeable working backlog.
