using System.Text.Json;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class ImportJobsFromProviderCommandHandler(
    JobSearchDbContext dbContext,
    IJobSearchProvider jobSearchProvider) : IRequestHandler<ImportJobsFromProviderCommand, ImportJobsResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<ImportJobsResponse> Handle(ImportJobsFromProviderCommand request, CancellationToken cancellationToken)
    {
        var providerRequest = new SearchJobsRequest(
            Location: request.Where,
            Keyword: request.Keyword,
            PageIndex: request.PageIndex,
            PageSize: request.PageSize,
            Provider: request.Provider,
            ExcludedKeyword: request.ExcludedKeyword,
            DistanceKilometers: request.DistanceKilometers,
            Category: request.Category,
            SalaryMin: request.SalaryMin,
            SalaryMax: request.SalaryMax,
            MaxDaysOld: request.MaxDaysOld);

        var filtersJson = JsonSerializer.Serialize(request, JsonOptions);
        var refreshRun = JobRefreshRun.Start(
            providerName: request.Provider.ToString(),
            countryCode: "gb",
            requestFiltersJson: filtersJson,
            requestedPageSize: request.PageSize,
            requestedMaxPages: 1);

        dbContext.JobRefreshRuns.Add(refreshRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var providerResult = await jobSearchProvider.SearchAsync(providerRequest, cancellationToken);
            refreshRun.RecordFetchedPage(providerResult.Jobs.Count);

            var importedJobs = providerResult.Jobs
                .Select(job => CloneImportedJob(job, refreshRun.Id))
                .ToArray();

            dbContext.Jobs.AddRange(importedJobs);
            refreshRun.RecordInsertedJobs(importedJobs.Length);
            refreshRun.RecordHiddenJobs(importedJobs.Count(job => job.IsHidden));
            refreshRun.Complete();

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ImportJobsResponse(
                JobRefreshRunId: refreshRun.Id,
                Source: request.Provider.ToString(),
                ImportedCount: importedJobs.Length,
                FailedCount: 0);
        }
        catch
        {
            refreshRun.RecordFailedItems(1);
            refreshRun.Fail("Provider import failed.");
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private static JobPosting CloneImportedJob(JobPosting source, long refreshRunId)
        => JobPosting.Create(
            refreshRunId,
            source.SourceName,
            source.SourceJobId,
            source.SourceAdReference,
            source.Title,
            source.Description,
            source.Summary,
            source.Url,
            source.Company,
            source.CompanyDisplayName,
            source.CompanyCanonicalName,
            source.Postcode,
            source.LocationName,
            source.LocationDisplayName,
            source.LocationAreaJson,
            source.Latitude,
            source.Longitude,
            source.CategoryTag,
            source.CategoryLabel,
            source.SalaryMin,
            source.SalaryMax,
            source.SalaryCurrency,
            source.SalaryIsPredicted,
            source.ContractTime,
            source.ContractType,
            source.IsFullTime,
            source.IsPartTime,
            source.IsPermanent,
            source.IsContract,
            source.IsRemote,
            source.PostedAtUtc,
            DateTime.UtcNow,
            DateTime.UtcNow,
            source.IsHidden,
            source.RawPayloadJson);
}
