# Product Requirements Document

## Product Name
Firefly Signal

## Product Summary
Firefly Signal is a personal career intelligence workspace for UK software-development job hunting.
It is being built first for the repository owner, but the product should still feel like a real, reviewable application rather than a throwaway personal script.

The MVP is intentionally close to the final product.
It should include the real end-to-end workflow needed to discover jobs, manage a personal job pipeline, store profile material, and use AI to support decisions.
The main capabilities that are intentionally deferred beyond the MVP are advanced user management and native mobile apps.

## Product Intent
- Help one user find relevant UK developer jobs efficiently.
- Keep the workflow persisted, reviewable, and practical for daily use.
- Build a codebase and web product that recruiters can inspect as evidence of product thinking, engineering quality, and maintainable structure.
- Preserve a path to broader future usage without forcing multi-tenant complexity into the MVP.

## Primary User
- Repository owner
- Personal-use first
- Looking specifically for UK developer jobs
- Comfortable with a web-first workflow

## Supported Geography And Job Focus
- Geography: United Kingdom only for the MVP
- Role focus: software-development jobs only for the MVP
- Location filtering must support distance-from-postcode search

## Supported Roles
### Admin
- Full frontend access
- Full backend write access
- Own profile, CV, and personal information
- Can import, manage, analyze, save, apply, and review jobs

### Test Admin
- Same frontend navigation and visible product surface as admin
- Read-only backend permissions for protected write operations
- Own separate profile, CV, and personal information
- Intended for demo and review access without permitting destructive or data-changing backend actions

## Documentation Source Of Truth
- `docs/product-requirements-document.md`
  Product scope, MVP definition, user workflows, and requirement boundaries
- `docs/plans.md`
  Delivery sequencing and phase-level planning
- `docs/frontend-designs.md`
  Frontend implementation direction and UI architecture
- `docs/backend-designs.md`
  Backend service direction, persistence boundaries, and integration design
- `docs/development/roadmap.md`
  Practical implementation order from the current repo baseline
- `docs/development/todo.md`
  Prioritized working backlog and candidate future issues

If there is a conflict, product scope should be resolved here first and then propagated to the implementation-facing docs.

## Product Vision
The final product should let the user:
- load jobs from external providers through public APIs or controlled scraping
- persist and manage a job catalog locally
- search, filter, and sort jobs effectively
- save jobs and move them through a personal application workflow
- maintain personal profile and CV information in the product
- attach submitted documents and notes to applied jobs
- use AI assistance to rate and explain job fit based on the user's stored profile material

The MVP should already include these core capabilities in a practical single-user form.

## MVP User Journeys
### 1. Load And Maintain Job Catalog
1. Admin triggers a job import from one or more providers.
2. System fetches job listings from public APIs or approved scraping flows.
3. System normalizes and stores jobs in the local database.
4. Admin can review imported jobs, hide bad records, edit records when needed, or delete records.

### 2. Search And Review Jobs
1. User enters a keyword.
2. User enters a UK postcode.
3. User optionally applies filters and sorting.
4. User filters by distance from the postcode.
5. System returns stored UK developer jobs that match the search and filter criteria.
6. User opens job details and can decide whether to save, apply, reject, or ignore the listing.

### 3. Manage Personal Job Workflow
1. User saves a job for later review.
2. User marks a job as applied.
3. User attaches the CV and cover letter used for that application.
4. User writes notes against the application.
5. User can later mark the application as rejected.
6. User can separately view saved jobs and applied jobs.

### 4. Manage Profile Material
1. User stores personal profile information in the product.
2. User uploads CVs and supporting profile documents.
3. System persists profile data and document metadata so later workflows can use them.
4. Admin and test admin each have their own separate stored profile data.

### 5. Run AI Assistance
1. Admin selects one or more jobs in an admin workflow.
2. Admin asks AI to analyze those jobs against the selected user's CV and profile data.
3. System sends the relevant job and user context to the AI service.
4. AI returns a suggested rating from 1 to 5 stars for each job.
5. Admin can also request detailed explanation and CV improvement guidance.
6. AI notes and ratings are linked to the user they were generated for.

## MVP Goals
- Provide a practical personal-use system for UK developer job discovery and tracking.
- Persist job data locally instead of depending only on transient search responses.
- Support a full personal workflow from job ingestion through application tracking.
- Support profile and document storage needed for real applications.
- Support AI-assisted rating and explanation as a bounded workflow tied to stored user context.
- Keep the codebase clean and reviewable for external code inspection.

## MVP Non-Goals
- Native mobile applications
- Advanced multi-user account management
- Complex tenant, team, or organization features
- Public self-serve sign-up flows for arbitrary users
- Generalized recruiting or ATS platform workflows
- Broad non-UK or non-developer job support

## Functional Requirements
### Authentication And Access
- System supports at least `admin` and `test-admin` roles.
- Admin can perform backend write operations.
- Test admin can access the same frontend areas but is blocked from backend mutations.
- Protected routes must reflect backend authorization decisions clearly.

### Job Ingestion And Storage
- System can load jobs from external providers through public APIs and, if needed later, controlled scraping.
- Imported jobs are stored in PostgreSQL.
- System keeps enough provider metadata to re-check, audit, and debug imported records.
- System supports admin CRUD operations for stored jobs.
- System supports hiding low-quality or unwanted jobs from normal search results.

### Search And Discovery
- User can search by keyword.
- User can search by UK postcode.
- User can filter by distance from the entered postcode.
- User can apply additional filters and sorting against stored jobs.
- Search results must stay constrained to UK developer jobs for the MVP.
- Search results must provide enough detail to judge relevance quickly.

### Job Detail
- Job detail view must show normalized job information and a path to the original listing.
- Job detail must support save, apply, reject, and AI-review-oriented workflows once those features exist.

### Personal Job Workflow
- User can save jobs.
- User can mark jobs as applied.
- User can mark jobs as rejected.
- User can view saved jobs separately from applied jobs.
- User can add notes to applied jobs.
- User can attach submitted CVs and cover letters to applied jobs.

### Profile And Document Management
- User can store personal profile information needed for AI-assisted fit review.
- User can upload CVs.
- User can upload other personal supporting documents where needed.
- User profile data and documents are stored per user.

### AI Assistance
- Admin can select jobs and run AI rating against a chosen user's stored profile context.
- AI must return a 1-5 star rating per job.
- Admin can request a more detailed explanation for selected jobs.
- AI can return CV improvement suggestions related to a job.
- AI notes and ratings are stored with user linkage so they remain attributable and reviewable later.

### Reliability And Reviewability
- Search must expose clear loading, empty, and error states.
- Protected workflows must expose permission failures clearly.
- The codebase must remain structured, understandable, and reviewable in small increments.
- Product behavior must avoid placeholder-only features that are not supported by real backend state.

## Core Data Expectations
- Jobs are persisted and manageable.
- Saved and applied states are persisted per user.
- Notes are persisted per application or job workflow record.
- Uploaded document metadata is persisted in the database.
- AI outputs are persisted and linked to both user and job context.

## Quality Attributes
- Clear and fast search workflow
- Low-friction personal job management
- Maintainable architecture for a single primary maintainer
- Good auditability of imported jobs and AI outputs
- Strong demo value for recruiter review of both UX and code

## Constraints
- Frontend hosted on Cloudflare
- Backend runs in Docker on a Mac mini
- API exposed using Cloudflare Tunnel
- Frontend is client-side rendered
- Backend stack is .NET 10, PostgreSQL, EF Core, RabbitMQ, and selective Redis
- Do not use .NET Aspire
- MVP is single-user oriented in practice, but should not be coded as an unstructured one-off

## Success Criteria For Early Releases
- User can import and manage UK developer jobs without leaving the product for core workflow steps.
- User can search persisted jobs by keyword and postcode distance without confusion.
- User can save, apply, reject, annotate, and review jobs in one coherent system.
- User can maintain profile material and use it in AI analysis flows.
- Admin and test admin flows are both demonstrable and clearly permissioned.
- The codebase remains clean enough to support recruiter review and future issue-by-issue delivery.

## Product Risks
- Public job source quality, coverage, terms, and rate limits may vary.
- Postcode-distance filtering depends on reliable geocoding or postcode reference data.
- File/document storage strategy can become messy if not bounded early.
- AI output quality may vary and must be presented as assistance rather than ground truth.
- Over-fragmenting services too early could slow delivery of the actual MVP workflow.

## Deliberately Deferred Decisions
- Exact provider order after the first live provider is integrated
- Exact long-term storage backend for document binaries beyond the MVP local-first path
- Whether scraping becomes necessary for provider coverage after public API evaluation
- Whether future non-admin roles beyond `test-admin` are justified

## Current Recommendation
Build the MVP as a real single-user product, not as a stripped-down demo.
Keep the scope broad enough to support the actual personal job workflow, while still avoiding advanced user-management and mobile-platform work.
