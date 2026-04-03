using Firefly.Signal.JobSearch.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContextSeed : IDbSeeder<JobSearchDbContext>
{
    public async Task SeedAsync(JobSearchDbContext context)
    {
        if (!await context.Jobs.AnyAsync())
        {
            context.Jobs.AddRange(
                JobPosting.Create(
                    ".NET Backend Developer",
                    "North Star Tech",
                    "SW1A 1AA",
                    "London",
                    "Build internal APIs with .NET 10, PostgreSQL and RabbitMQ.",
                    "https://example.com/jobs/backend-dotnet",
                    "sample-feed",
                    true,
                    DateTime.UtcNow.AddDays(-1)),
                JobPosting.Create(
                    "Full Stack React and .NET Engineer",
                    "Signal Works",
                    "M1 1AE",
                    "Manchester",
                    "Own a React frontend and .NET backend for a hiring analytics product.",
                    "https://example.com/jobs/full-stack",
                    "sample-feed",
                    false,
                    DateTime.UtcNow.AddDays(-2)),
                JobPosting.Create(
                    "Senior Platform Engineer",
                    "Cloud Rail",
                    "EH1 1YZ",
                    "Edinburgh",
                    "Help modernize event-driven services and CI/CD pipelines.",
                    "https://example.com/jobs/platform-engineer",
                    "sample-feed",
                    true,
                    DateTime.UtcNow.AddDays(-3)));

            await context.SaveChangesAsync();
        }
    }
}
