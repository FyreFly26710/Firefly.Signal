# Firefly Signal Plan

## Planning Scope
This document captures the current execution plan for Firefly Signal from the repository's real baseline.
It is intentionally biased toward a practical personal-use MVP that already includes most of the intended final product workflow.

The main features intentionally left beyond the MVP are:
- advanced user management
- native mobile applications

## Working Product Assumptions
- The product is primarily for the repository owner's personal use.
- The MVP should still behave like a real product that recruiters can review.
- The MVP should include persisted job data, job workflow management, profile data, document linkage, and bounded AI assistance.
- The first supported geography is the United Kingdom only.
- The first supported role category is developer jobs only.
- The frontend is client-side only and will talk to backend APIs through a Cloudflare-exposed entry point.
- The backend will run on a Mac mini in Docker behind Cloudflare Tunnel.

## Goals For The Current Planning Phase
- Keep the product requirements and planning docs aligned.
- Deliver the full personal job-search workflow in small, reviewable issues.
- Keep backend boundaries explicit while avoiding premature service sprawl.
- Preserve strong code review quality for both maintainability and recruiter visibility.

## Recommended Delivery Phases
### Phase 0: Product And Planning Alignment
- Clarify the PRD and source-of-truth documentation boundaries.
- Update roadmap and backlog docs to match the actual MVP.
- Keep the project explanation docs aligned with the current repo shape.

### Phase 1: Identity, Roles, And Profile Foundations
- Keep the existing auth foundation and formalize role behavior for `admin` and `test-admin`.
- Add user profile persistence for personal information.
- Add document metadata and upload handling for CVs and other profile documents.
- Ensure test admin matches admin frontend visibility while remaining backend read-only.

### Phase 2: Persisted Job Catalog And Admin Management
- Make persisted job storage a first-class MVP capability.
- Support provider-backed import runs with normalization and deduplication.
- Support admin CRUD, moderation, and catalog review workflows.
- Preserve source payload metadata for auditability and future remapping.

### Phase 3: Search, Filtering, Sorting, And Distance
- Deliver search over the persisted job catalog.
- Support keyword, postcode, distance, sorting, and practical filtering.
- Keep the experience constrained to UK developer jobs.
- Maintain clear loading, empty, error, and permission states.

### Phase 4: Personal Job Workflow
- Add save-job, applied-job, and rejected-job states.
- Add views for saved jobs and applied jobs.
- Support notes on applied jobs.
- Support attaching submitted CVs and cover letters to applications.

### Phase 5: AI-Assisted Review
- Let admin select jobs and run AI rating against stored user profile and CV context.
- Return a 1-5 star rating per job.
- Support optional detailed explanation and CV improvement guidance.
- Persist AI outputs and keep them linked to the user they were generated for.

### Phase 6: Recruiter-Visible Hardening
- Improve code quality and operational clarity where needed.
- Refine the UI and admin/test-admin flows for demonstration quality.
- Keep product and planning docs aligned with the implemented workflow.

## Repository Shape To Grow Into
```text
apps/
  web/
docs/
  backend-designs.md
  development/
  frontend-designs.md
  plans.md
  product-requirements-document.md
services/
  api/
infra/
.github/
  workflows/
```

## Delivery Principles
- Prefer vertical slices over abstract platform work.
- Keep each issue small enough for one focused branch and one reviewable PR.
- Use real persisted state where the MVP requires it instead of relying on placeholder workflows.
- Keep docs focused on product, planning, and explaining the current system.
- Keep implementation choices easy to explain in a code-review context.

## Proposed Issue Themes From Here
1. Clarify PRD and source-of-truth docs
2. Add user profile and CV and document metadata persistence
3. Formalize `admin` and `test-admin` authorization behavior
4. Persist imported jobs and deduplicate provider records
5. Add admin workflows for job CRUD and moderation
6. Add postcode distance filtering and sorting over stored jobs
7. Add saved-job and applied-job persistence
8. Add application notes and submitted-document linkage
9. Add AI job rating workflow linked to user profile context
10. Harden frontend and backend quality around the end-to-end workflow

## Risks To Manage Early
- Under-modeling the personal workflow and later patching it with ad hoc fields
- Overcomplicating service boundaries before the workflow is fully implemented
- Letting document storage and linkage become inconsistent
- Building AI features without clear user-owned context and auditability
- Letting product and planning docs lag behind the actual intended MVP

## Success Criteria For The Planning Phase
- Product direction is explicit enough to create focused issues without re-deciding scope.
- MVP boundaries are clear across the PRD, plan, and development docs.
- The backlog is prioritized around the real personal-use workflow rather than a smaller demo product.

## Notes For Future Planning
- Read the PRD first for scope.
- Then read this file for sequencing.
- Then read `docs/development/roadmap.md` and `docs/development/todo.md` for practical issue planning.
- Update these docs whenever durable product or sequencing decisions change.
