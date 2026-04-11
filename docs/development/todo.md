# Development Todo

This file captures the prioritized working backlog that is not yet fully represented as completed GitHub issues or roadmap phases.
It should stay practical, easy to scan, and easy to convert into focused issues.

## How To Use This File

- Keep tasks grouped by priority.
- Prefer issue-sized items over vague idea dumping.
- Remove an item once it becomes an active GitHub issue or completed work.
- Move sequencing-level changes into `roadmap.md` when they become durable.

## Priority Levels

- `P0`
  Required to deliver the real MVP workflow
- `P1`
  Important follow-up work that strengthens the MVP soon after the core slice
- `P2`
  Valuable later work that should wait until the MVP loop is stable

## P0: Core MVP Work

- Clarify product requirements and source-of-truth docs.
- Formalize backend authorization for `admin` and `test-admin`.
- Add persisted user profiles for admin and test admin.
- Add CV and profile-document metadata plus upload flow.
- Persist imported jobs and deduplicate repeated provider listings.
- Harden admin CRUD and moderation workflows for stored jobs.
- Add postcode-distance filtering for UK postcode search.
- Add richer filters and sorting over stored jobs.
- Add saved-job persistence.
- Add applied-job persistence and rejected-job state.
- Add notes on applied jobs.
- Add submitted CV and cover-letter linkage for applications.
- Add separate views for saved jobs and applied jobs.
- Add AI job-fit ratings from 1 to 5 stars for selected jobs.
- Add AI detailed explanation and CV-improvement guidance linked to a user.

## P1: Important Near-Term Enhancements

- Add provider-run history and admin visibility for imports.
- Add postcode lookup/geocoding strategy hardening and caching.
- Add safer document storage implementation details and retention rules.
- Add better operational observability around provider imports and AI analysis.
- Add bulk admin actions for save/apply/reject/moderation flows where they improve review speed.
- Add more explicit audit history for job moderation and AI analysis runs.
- Improve recruiter-facing UX polish for admin and test-admin review sessions.
- Expand functional tests around auth, workflow state, and protected permissions.

## P2: Later Work

- Add a second provider or fallback provider for better result coverage.
- Add scheduled job collection and refresh automation.
- Add search history and saved-search automation.
- Add export flows for saved jobs, applied jobs, or AI outputs.
- Add notification ideas for newly matching jobs.
- Add interview-preparation workspace features.
- Add broader insights or reporting dashboards.
- Add Google OAuth if it still adds value after the MVP auth flow is stable.
- Revisit service boundaries if real MVP friction justifies splitting profile, document, or AI workflow concerns.

## Candidate Issue Shapes

- `P0`: Add user profile entity and endpoints in Identity API
- `P0`: Add user document metadata and local file-upload workflow
- `P0`: Add postcode-distance filtering to persisted job search
- `P0`: Add user job state for saved, applied, and rejected jobs
- `P0`: Add application notes and submitted-document linkage
- `P0`: Add AI job-fit scoring endpoint and persistence
- `P1`: Add import-run history and provider diagnostics
- `P1`: Add test-admin permission coverage across frontend and backend
- `P2`: Add scheduled refresh for tracked searches

## Notes

- Prioritize the real single-user workflow over optional platform expansion.
- Do not reintroduce placeholder-only features when the backend workflow is not ready.
- Keep the product constrained to UK developer-job hunting until the MVP is proven.
