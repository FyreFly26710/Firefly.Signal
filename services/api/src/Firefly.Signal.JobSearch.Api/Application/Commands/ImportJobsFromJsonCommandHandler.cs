using System.Text.Json;
using Firefly.Signal.JobSearch.Application.Exceptions;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class ImportJobsFromJsonCommandHandler(JobSearchDbContext dbContext)
    : IRequestHandler<ImportJobsFromJsonCommand, ImportJobsResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<ImportJobsResponse> Handle(ImportJobsFromJsonCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.JsonStream);

        var payload = await DeserializePayloadAsync(request.JsonStream, cancellationToken);

        if (payload?.Jobs is null || payload.Jobs.Count == 0)
        {
            throw new JobImportPayloadException($"The uploaded file '{request.FileName}' does not contain any jobs.");
        }

        var refreshRun = JobRefreshRun.Start(
            providerName: "json-upload",
            countryCode: "gb",
            requestFiltersJson: JsonSerializer.Serialize(new { request.FileName, JobCount = payload.Jobs.Count }, JsonOptions),
            requestedPageSize: payload.Jobs.Count,
            requestedMaxPages: 1);

        dbContext.JobRefreshRuns.Add(refreshRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            refreshRun.RecordFetchedPage(payload.Jobs.Count);

            var importedJobs = payload.Jobs
                .Select(job => CreateImportedJob(job, refreshRun.Id))
                .ToArray();

            dbContext.Jobs.AddRange(importedJobs);
            refreshRun.RecordInsertedJobs(importedJobs.Length);
            refreshRun.RecordHiddenJobs(importedJobs.Count(job => job.IsHidden));
            refreshRun.Complete();

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ImportJobsResponse(
                JobRefreshRunId: refreshRun.Id,
                Source: "json-upload",
                ImportedCount: importedJobs.Length,
                FailedCount: 0);
        }
        catch
        {
            refreshRun.RecordFailedItems(1);
            refreshRun.Fail("JSON import failed.");
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<ImportedJobsFile?> DeserializePayloadAsync(
        Stream jsonStream,
        CancellationToken cancellationToken)
    {
        try
        {
            return await JsonSerializer.DeserializeAsync<ImportedJobsFile>(
                jsonStream,
                JsonOptions,
                cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new JobImportPayloadException(
                "The uploaded file is not valid job export JSON.",
                exception);
        }
    }

    private static JobPosting CreateImportedJob(CreateJobRequest request, long refreshRunId)
        => JobPosting.Create(
            refreshRunId,
            request.SourceName,
            request.SourceJobId,
            request.SourceAdReference,
            request.Title,
            request.Description,
            request.Summary,
            request.Url,
            request.Company,
            request.CompanyDisplayName,
            request.CompanyCanonicalName,
            request.Postcode,
            request.LocationName,
            request.LocationDisplayName,
            request.LocationAreaJson,
            request.Latitude,
            request.Longitude,
            request.CategoryTag,
            request.CategoryLabel,
            request.SalaryMin,
            request.SalaryMax,
            request.SalaryCurrency,
            request.SalaryIsPredicted,
            request.ContractTime,
            request.ContractType,
            request.IsFullTime,
            request.IsPartTime,
            request.IsPermanent,
            request.IsContract,
            request.IsRemote,
            request.PostedAtUtc,
            request.ImportedAtUtc == default ? DateTime.UtcNow : request.ImportedAtUtc,
            request.LastSeenAtUtc == default ? DateTime.UtcNow : request.LastSeenAtUtc,
            request.IsHidden,
            request.RawPayloadJson);
}
