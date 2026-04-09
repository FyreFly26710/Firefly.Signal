using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

public sealed class MockAdzunaJobSearchProvider : IJobSearchProvider
{
    public JobSearchProviderKind Provider => JobSearchProviderKind.Adzuna;

    public Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
    {
        JobPosting[] jobs =
        [
            JobPosting.Create(
                $"{request.Keyword} Developer",
                "Mock North Star Tech",
                request.Postcode,
                "London",
                "Mock Adzuna result for local development and rate-limit-safe testing.",
                "https://example.com/jobs/mock-adzuna-1",
                "Adzuna",
                false,
                new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)),
            JobPosting.Create(
                $"{request.Keyword} Platform Engineer",
                "Mock Cloud Rail",
                request.Postcode,
                "Manchester",
                "Mock Adzuna result for provider wiring and UI verification.",
                "https://example.com/jobs/mock-adzuna-2",
                "Adzuna",
                true,
                new DateTime(2026, 4, 2, 11, 30, 0, DateTimeKind.Utc))
        ];

        return Task.FromResult<PublicJobSearchResult>(new PublicJobSearchResult(jobs.LongLength, jobs));
    }
}
