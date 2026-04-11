# Backend Data Model Plan

This document captures the recommended MVP data model shape for Firefly Signal based on the current product requirements.
It is a planning document, not a migration contract.

## Goals
- Model the real personal-use workflow directly.
- Keep ownership boundaries understandable inside the current service layout.
- Support small, reviewable implementation issues without painting the product into a corner.

## Service Ownership Recommendation
### Identity API
Own:
- user accounts
- roles and permission claims
- user profile information
- user-owned document metadata

### Job Search API
Own:
- provider refresh runs
- imported job postings
- search and filtering support data
- user job workflow state such as saved, applied, and rejected
- application notes
- application-to-document linkage
- persisted AI outputs linked to jobs and users

### AI API
Own:
- execution logic and prompt orchestration
- request and response contracts for job-fit analysis

Do not make the AI API the system of record for persisted AI outcomes.
Persist the durable result records in the Job Search API domain.

## Identity Domain Entities
### UserAccount
Purpose:
- authentication identity and role ownership

Core fields:
- `Id`
- `UserAccountName`
- `PasswordHash`
- `Email`
- `DisplayName`
- `Role`
- audit fields

Notes:
- roles should support at least `admin` and `test-admin`
- `test-admin` should be treated as read-only for backend mutations

### UserProfile
Purpose:
- store the user's personal context for job selection and AI assistance

Core fields:
- `Id`
- `UserAccountId`
- `FullName`
- `PreferredTitle`
- `PrimaryLocationPostcode`
- `LinkedInUrl`
- `GithubUrl`
- `PortfolioUrl`
- `Summary`
- `SkillsText`
- `ExperienceText`
- `PreferencesJson`
- audit fields

Notes:
- keep the initial profile flexible
- avoid over-normalizing profile details until real usage proves the need

### UserDocument
Purpose:
- store metadata for uploaded CVs, cover letters, and supporting documents

Core fields:
- `Id`
- `UserAccountId`
- `DocumentType`
- `DisplayName`
- `OriginalFileName`
- `StorageKey`
- `ContentType`
- `FileSizeBytes`
- `ChecksumSha256`
- `IsDefault`
- `UploadedAtUtc`
- audit fields

Recommended `DocumentType` values:
- `cv`
- `cover-letter`
- `profile-supporting`
- `other`

Notes:
- store binary files outside the relational row
- keep metadata in PostgreSQL
- use the `StorageKey` as the Amazon S3 object key for the uploaded file

## Job Search Domain Entities
### JobRefreshRun
Purpose:
- track each import or refresh operation from a provider

Core fields:
- `Id`
- `RequestedByUserAccountId`
- `ProviderKind`
- `SourceMode` (`api` or `scrape`)
- `Status`
- `StartedAtUtc`
- `CompletedAtUtc`
- `ImportedCount`
- `UpdatedCount`
- `HiddenCount`
- `FailureSummary`
- audit fields

### JobPosting
Purpose:
- normalized persisted job record

Existing code already contains most of this shape.

Additional planning considerations:
- add a durable deduplication key
- preserve provider identity and raw payload
- keep latitude and longitude when available
- keep postcode and normalized location fields to support postcode-distance filtering

Potential future fields:
- `DeduplicationKey`
- `SalaryText`
- `LastAnalyzedAtUtc`

### PostcodeLookupCache
Purpose:
- support postcode-distance filtering

Core fields:
- `Id`
- `Postcode`
- `Latitude`
- `Longitude`
- `Source`
- `LastVerifiedAtUtc`

Notes:
- this may be a table, cache, or imported reference dataset depending on implementation choice
- the important requirement is that postcode-to-coordinate resolution is reliable enough for search filtering

### UserJobState
Purpose:
- store per-user relationship to a job in the personal workflow

Core fields:
- `Id`
- `UserAccountId`
- `JobPostingId`
- `State`
- `SavedAtUtc`
- `AppliedAtUtc`
- `RejectedAtUtc`
- `LastUpdatedAtUtc`
- audit fields

Recommended `State` values:
- `saved`
- `applied`
- `rejected`

Notes:
- this should be user-specific
- avoid overloading the core `JobPosting` row with user workflow state

### JobApplication
Purpose:
- hold richer application details once a user has applied

Core fields:
- `Id`
- `UserAccountId`
- `JobPostingId`
- `AppliedAtUtc`
- `Status`
- `SubmittedCvDocumentId`
- `SubmittedCoverLetterDocumentId`
- `RejectionAtUtc`
- `RejectionReason`
- audit fields

Recommended `Status` values:
- `applied`
- `rejected`

Notes:
- this can coexist with `UserJobState`
- keep it focused on applied-job metadata rather than becoming a full ATS system

### ApplicationNote
Purpose:
- store personal notes attached to an application

Core fields:
- `Id`
- `JobApplicationId`
- `UserAccountId`
- `Body`
- `CreatedAtUtc`
- `UpdatedAtUtc`

### ApplicationDocumentLink
Purpose:
- link additional user-owned documents to an application

Core fields:
- `Id`
- `JobApplicationId`
- `UserDocumentId`
- `LinkType`
- audit fields

Recommended `LinkType` values:
- `submitted-cv`
- `submitted-cover-letter`
- `supporting-document`

### UserJobAiInsight
Purpose:
- persist AI-generated job-fit results linked to a specific user and job

Core fields:
- `Id`
- `UserAccountId`
- `JobPostingId`
- `GeneratedByUserAccountId`
- `Rating`
- `Summary`
- `DetailedExplanation`
- `CvImprovementSuggestions`
- `PromptVersion`
- `GeneratedAtUtc`
- audit fields

Rules:
- `Rating` must be constrained to 1 through 5
- results are user-specific
- results should be treated as assistance, not authoritative truth

### AiAnalysisRun
Purpose:
- track bulk or multi-job AI requests initiated from admin workflows

Core fields:
- `Id`
- `RequestedByUserAccountId`
- `TargetUserAccountId`
- `Mode`
- `Status`
- `JobCount`
- `StartedAtUtc`
- `CompletedAtUtc`
- `FailureSummary`

Recommended `Mode` values:
- `rating`
- `detailed`
- `cv-improvement`

## Key Relationships
- `UserAccount` 1-to-1 `UserProfile`
- `UserAccount` 1-to-many `UserDocument`
- `JobRefreshRun` 1-to-many `JobPosting`
- `UserAccount` 1-to-many `UserJobState`
- `JobPosting` 1-to-many `UserJobState`
- `UserAccount` 1-to-many `JobApplication`
- `JobPosting` 1-to-many `JobApplication`
- `JobApplication` 1-to-many `ApplicationNote`
- `JobApplication` 1-to-many `ApplicationDocumentLink`
- `UserDocument` 1-to-many `ApplicationDocumentLink`
- `UserAccount` 1-to-many `UserJobAiInsight`
- `JobPosting` 1-to-many `UserJobAiInsight`
- `AiAnalysisRun` 1-to-many `UserJobAiInsight`

## Boundary Rules
- Keep auth and profile ownership in Identity.
- Keep persisted job and personal workflow state in Job Search.
- Keep AI execution logic in AI, but persist durable AI outputs in Job Search.
- Use user IDs as explicit cross-service references instead of hiding them behind vague shared abstractions.

## Sequencing Recommendation
Implement in this order:
1. `UserProfile`
2. `UserDocument`
3. `JobRefreshRun` hardening and `JobPosting` persistence/deduplication
4. `UserJobState`
5. `JobApplication`
6. `ApplicationNote`
7. `ApplicationDocumentLink`
8. `UserJobAiInsight`
9. `AiAnalysisRun`

## Current Recommendation
Use a small set of explicit, user-owned workflow entities rather than trying to encode the whole product inside `JobPosting` or `UserAccount`.
That will keep the MVP understandable and make later issues easier to scope cleanly.
