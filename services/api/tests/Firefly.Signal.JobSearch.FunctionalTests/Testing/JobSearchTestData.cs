using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.FunctionalTests.Testing;

internal static class JobSearchTestData
{
    public static JobPosting CreateJob(
        string title,
        DateTime postedAtUtc,
        bool isHidden = false)
    {
        var job = JobPosting.Create(
            title: title,
            company: "Firefly Labs",
            postcode: "sw1a 1aa",
            locationName: "London",
            summary: $"{title} summary",
            url: $"https://example.test/jobs/{title.Replace(' ', '-').ToLowerInvariant()}",
            sourceName: "adzuna",
            isRemote: true,
            postedAtUtc: postedAtUtc);

        if (isHidden)
        {
            job.Hide();
        }

        return job;
    }
}
