# Adzuna API Notes

This document captures the practical Adzuna integration guidance for Firefly Signal.
It supplements the general backend design docs with provider-specific behavior, constraints, and repository choices.

## Official References

- Overview: `https://developer.adzuna.com/overview`
- Search docs: `https://developer.adzuna.com/docs/search`
- Terms of service: `https://developer.adzuna.com/docs/terms_of_service`
- Interactive docs: `https://developer.adzuna.com/activedocs?`
- OpenAPI spec: `https://developer.adzuna.com/swagger/spec/test2.json`

## Endpoint Shape

The main search endpoint is:

```text
GET https://api.adzuna.com/v1/api/jobs/{country}/search/{page}
```

For Firefly Signal, the current country is `gb`.

Required query parameters:

- `app_id`
- `app_key`

Common query parameters used in this repo:

- `results_per_page`
- `what`
- `what_exclude`
- `where`
- `distance`
- `category`
- `salary_min`
- `salary_max`
- `full_time`
- `part_time`
- `permanent`
- `contract`
- `sort_by`
- `max_days_old`
- `company`
- `title_only`
- `location0`

## Response Notes

The response includes:

- `count` for total matching results
- `results` for the current page
- job-level fields such as `id`, `title`, `description`, `redirect_url`, `created`
- nested `company` and `location` objects

Adzuna returns only a snippet of the job description rather than a full original advert body.

## Practical Findings For Firefly Signal

Manual checks against the live UK search endpoint showed:

- `results_per_page` is practically capped at `50`
- values above `50` still return `200`, but only `50` jobs are returned
- `what_exclude` worked in live calls
- `distance`, `category`, `salary_min`, `salary_max`, `full_time`, `part_time`, `permanent`, `contract`, `sort_by`, `max_days_old`, and `company` all worked in live calls
- `title_only=1` worked
- `title_only=0` was not reliable and should not be sent
- `location0=UK` worked in live calls
- the tested `location1` and `location2` combinations returned `400`, so they are currently not sent by the provider

## Rate Limits And Terms

Default Adzuna API access limits are:

- `25` hits per minute
- `250` hits per day
- `1000` hits per week
- `2500` hits per month

These limits come from the Adzuna terms of service.

Important usage constraints from the official terms:

- Adzuna may be used for publishing listings, salary estimates, and personal research
- commercial, government, or academic usage beyond the stated trial period may require written consent or a licence agreement
- published listings must include Adzuna attribution requirements

Any production or public-facing Firefly Signal usage should be reviewed against the latest Adzuna terms before launch.

## Repository Integration Choice

To avoid burning Adzuna rate limits during routine development:

- Firefly Signal uses a mock Adzuna provider by default
- live Adzuna calls are opt-in through configuration
- backend unit and functional tests do not call the real Adzuna API
- admin-triggered provider import flows should continue to work against the mock provider in local development

Current configuration behavior:

- `Adzuna:UseLiveApi = false`
  Uses the mock provider
- `Adzuna:UseLiveApi = true`
  Uses the live provider and requires valid `AppId` and `AppKey`

## Implementation Guidance

- Keep shared search request and response models provider-agnostic
- Keep Adzuna-specific request mapping and response mapping inside the Adzuna provider folder
- Prefer explicit mappers over leaking Adzuna field names into the broader application
- Do not assume Adzuna accepts every documented field combination without verifying the live behavior
- Be conservative about adding new Adzuna query parameters to the live request until manually verified
