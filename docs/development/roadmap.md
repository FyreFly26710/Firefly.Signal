# Development Roadmap

This roadmap translates the repository's current state and intended direction into a practical implementation sequence.
It is intentionally biased toward delivering the real personal-use MVP, not a narrow demo that will later need to be rebuilt.

## Planning Assumptions

- The web app is the primary client for the first release.
- The current codebase already contains an initial frontend search flow and backend service shells.
- The MVP should include most of the intended final workflow, except advanced user management and native mobile apps.
- The product is for UK developer-job discovery and management.
- Authentication, persisted job data, job workflow state, profile data, document linkage, and bounded AI support all belong to the MVP.
- The repository owner remains the primary user and reviewer.

## Current State Summary

The repo already has:
- a web app with public search, login, protected routes, and admin job-management screens
- a gateway API and downstream identity, job search, and AI APIs
- current domain models for user accounts and job postings
- local Docker and test infrastructure

The roadmap should therefore focus on turning that baseline into the full personal-use workflow rather than re-scaffolding the platform.

## Recommended Delivery Sequence

### Phase 1: Clarify Product Scope And Data Direction

Goal:
Align the PRD, planning docs, and backend data model with the real MVP.

Recommended issue themes:
- clarify MVP boundaries in the PRD
- add a data model plan for profile, documents, workflow state, and AI outcomes
- update roadmap and backlog docs to match the real product intent

### Phase 2: Formalize Roles, Profiles, And Document Metadata

Goal:
Make `admin` and `test-admin` behavior explicit and persist user profile context needed for later workflows.

Recommended issue themes:
- formalize backend authorization for `admin` and `test-admin`
- add user profile persistence
- add user document metadata for CVs and supporting documents
- expose profile read/update endpoints and initial frontend screens

### Phase 3: Persisted Job Catalog And Admin Operations

Goal:
Turn imported jobs into a durable catalog that can be reviewed and managed.

Recommended issue themes:
- persist provider-backed import runs and normalized jobs
- add deduplication rules and provider auditability
- improve admin CRUD and moderation flows
- keep provider metadata and raw payload available for review

### Phase 4: Search, Sorting, And Postcode Distance

Goal:
Make stored-job search genuinely useful for UK developer-job discovery.

Recommended issue themes:
- add keyword, sorting, and richer filter support over stored jobs
- implement postcode-distance filtering
- harden search validation, loading, empty, and error states
- keep the search loop clearly constrained to UK developer jobs

### Phase 5: Personal Job Workflow

Goal:
Support the real save/apply/reject workflow inside the product.

Recommended issue themes:
- add saved-job persistence
- add applied-job and rejected-job state
- add notes on applications
- add submitted-document linkage for applications
- add saved-jobs and applied-jobs views

### Phase 6: AI-Assisted Review

Goal:
Add bounded AI support that helps evaluate jobs against stored user context.

Recommended issue themes:
- allow admin to select jobs and run AI rating in bulk
- return 1-5 star fit scores
- support detailed explanation and CV-improvement guidance
- persist AI outputs linked to both user and job

### Phase 7: Recruiter-Facing Hardening

Goal:
Make the codebase and product stronger for ongoing use and external review.

Recommended issue themes:
- refine permissions and demo flows for test admin
- improve operational visibility and error handling
- strengthen tests around the main vertical slices
- improve UX polish where it clarifies product quality without broad redesign

## How To Turn This Roadmap Into Issues

Each issue should:
- target one concrete user or maintainer outcome
- touch one main concern where possible
- fit into one focused branch
- include acceptance criteria that can be validated locally

Good issue examples from this roadmap:
- add user profile persistence for admin and test admin
- persist uploaded CV metadata and expose profile document endpoints
- add postcode-distance filtering to stored-job search
- add saved and applied job workflow state
- add AI job-fit ratings linked to a user profile

## Roadmap Maintenance Rule

Update this roadmap when:
- the recommended sequencing changes materially
- a major phase has been completed
- a new product constraint changes what should happen next

Do not update it for every small implementation detail.
Use `todo.md` for the more changeable working backlog.
