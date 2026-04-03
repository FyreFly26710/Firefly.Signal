# Product Requirements Document

## Product Name
Firefly Signal

## Product Summary
Firefly Signal is a personal career intelligence platform.
Its first milestone is a web application that helps a user discover relevant UK jobs by entering a postcode and keyword, then reviewing clear search results gathered from public sources.

## Problem Statement
Job discovery is fragmented and repetitive.
The user wants a single place to search by location and intent, review relevant opportunities quickly, and eventually build a richer personal workflow around saved jobs, resume fit, AI assistance, and interview preparation.

## Primary User
- Repository owner
- Personal-use first
- Technically capable and comfortable evolving the product over time

## Initial Use Case
1. User opens the web app.
2. User enters a UK postcode.
3. User enters a job keyword.
4. User submits the search.
5. System retrieves relevant job opportunities from public sources.
6. System displays clear, useful results.

## MVP Goals
- Provide useful job search results for UK-based discovery.
- Make search quick and repeatable.
- Keep the product lightweight and easy to maintain.
- Establish a foundation for future persistence and AI assistance.

## MVP Non-Goals
- Native mobile applications
- Enterprise or multi-tenant account management
- Complex social or collaboration features
- Broad dashboarding before core search works well
- Heavy AI features before the core data flow is valuable

## Functional Requirements
### Search Input
- User can enter a UK postcode.
- User can enter a job keyword or phrase.
- System validates obviously invalid input.

### Search Execution
- System queries one or more public job data sources.
- System returns results relevant to the requested keyword and location intent.
- System handles slow or failed provider responses gracefully.

### Search Results
- System displays a list of jobs with enough information to assess relevance quickly.
- Each result should include, where available:
  - job title
  - company or employer
  - location
  - source
  - summary snippet
  - link or path to original listing
  - freshness or posted date

### Reliability
- System provides a clear error state when search fails.
- System provides a clear empty state when no results are found.
- System should be simple to operate and debug by one maintainer.

## Future Requirements
- Saved jobs
- Search history
- Job tracking pipeline
- Resume upload and analysis
- Personalized ranking and recommendation
- Cover letter drafting
- Interview preparation
- Insights and reporting

## Quality Attributes
- Fast perceived response time
- Clear UX with low friction
- Maintainable architecture
- Good observability for debugging
- Secure authentication model for future protected workflows

## Constraints
- Frontend hosted on Cloudflare
- Backend runs in Docker on a Mac mini
- API exposed using Cloudflare Tunnel
- Frontend is client-side rendered
- Backend stack is .NET 10, PostgreSQL, EF Core, RabbitMQ, and selective Redis
- Do not use .NET Aspire

## Success Metrics For Early Releases
- User can complete a search without confusion.
- Returned results are relevant enough to justify repeated use.
- The product remains easy to change issue by issue.
- The first end-to-end slice can be deployed and tested without heavy manual work.

## Product Risks
- Public job source quality or access may vary.
- Over-building infrastructure may slow actual user value.
- Search relevance may depend heavily on source quality and normalization choices.

## Open Questions
- Which job sources should be prioritized first?
- How much search history or persistence is valuable in the first release?
- Should auth be required for the first MVP, or introduced immediately after the search loop works?

## Current Recommendation
Build the smallest useful search loop first.
Once search quality and usability are proven, add persistence, personalization, and AI-assisted workflows incrementally.
