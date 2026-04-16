using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

public sealed class JobRefreshRun : AuditableEntity
{
    private JobRefreshRun()
    {
    }

    // Provider targeted by this admin-triggered refresh.
    public string ProviderName { get; private set; } = string.Empty;
    // Country scope sent to the provider request.
    public string CountryCode { get; private set; } = string.Empty;
    // Full set of admin-selected refresh filters stored as JSON.
    public string RequestFiltersJson { get; private set; } = "{}";
    // Requested provider page size for this run.
    public int RequestedPageSize { get; private set; }
    // Optional upper limit on the number of pages fetched in this run.
    public int? RequestedMaxPages { get; private set; }
    // Overall refresh lifecycle status for admin visibility.
    public JobRefreshRunStatus Status { get; private set; }
    // Number of provider pages attempted.
    public int PagesRequested { get; private set; }
    // Number of provider pages fetched successfully.
    public int PagesCompleted { get; private set; }
    // Total number of provider records returned across all fetched pages.
    public int RecordsReceived { get; private set; }
    // Number of jobs inserted into the local jobs table.
    public int RecordsInserted { get; private set; }
    // Number of inserted jobs currently marked hidden.
    public int RecordsHidden { get; private set; }
    // Number of items that could not be imported cleanly.
    public int RecordsFailed { get; private set; }
    // Last failure summary for partial or failed runs.
    public string FailureMessage { get; private set; } = string.Empty;
    // Timestamp when the refresh started.
    public DateTime StartedAtUtc { get; private set; }
    // Timestamp when the refresh completed or failed.
    public DateTime? CompletedAtUtc { get; private set; }

    public static JobRefreshRun Start(
        string providerName,
        string countryCode,
        string requestFiltersJson,
        int requestedPageSize,
        int? requestedMaxPages)
    {
        return new JobRefreshRun
        {
            ProviderName = providerName.Trim(),
            CountryCode = countryCode.Trim().ToLowerInvariant(),
            RequestFiltersJson = string.IsNullOrWhiteSpace(requestFiltersJson) ? "{}" : requestFiltersJson.Trim(),
            RequestedPageSize = requestedPageSize,
            RequestedMaxPages = requestedMaxPages,
            Status = JobRefreshRunStatus.Running,
            StartedAtUtc = DateTime.UtcNow
        };
    }

    public void RecordFetchedPage(int recordCount)
    {
        PagesRequested += 1;
        PagesCompleted += 1;
        RecordsReceived += recordCount;
        Touch();
    }

    public void RecordInsertedJobs(int insertedCount)
    {
        RecordsInserted += insertedCount;
        Touch();
    }

    public void RecordHiddenJobs(int hiddenCount)
    {
        RecordsHidden += hiddenCount;
        Touch();
    }

    public void RecordFailedItems(int failedCount)
    {
        RecordsFailed += failedCount;
        Touch();
    }

    public void Complete(bool partial = false)
    {
        Status = partial ? JobRefreshRunStatus.PartiallyCompleted : JobRefreshRunStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Fail(string failureMessage)
    {
        Status = JobRefreshRunStatus.Failed;
        FailureMessage = failureMessage.Trim();
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }
}
