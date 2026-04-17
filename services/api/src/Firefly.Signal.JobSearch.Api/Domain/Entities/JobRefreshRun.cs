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
    public bool IsTerminal => Status != JobRefreshRunStatus.Running;
    public bool HasFailures => RecordsFailed > 0 || !string.IsNullOrWhiteSpace(FailureMessage);
    public int VisibleRecordsInserted => Math.Max(0, RecordsInserted - RecordsHidden);
    public string? FailureSummary => string.IsNullOrWhiteSpace(FailureMessage) ? null : FailureMessage;

    public static JobRefreshRun Start(
        string providerName,
        string countryCode,
        string requestFiltersJson,
        int requestedPageSize,
        int? requestedMaxPages)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            throw new ArgumentException("Provider name is required.", nameof(providerName));
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException("Country code is required.", nameof(countryCode));
        }

        if (requestedPageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedPageSize), "Requested page size must be greater than zero.");
        }

        if (requestedMaxPages <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedMaxPages), "Requested max pages must be greater than zero when provided.");
        }

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
        EnsureRunning();

        if (recordCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(recordCount), "Fetched record count cannot be negative.");
        }

        PagesRequested += 1;
        PagesCompleted += 1;
        RecordsReceived += recordCount;
        Touch();
    }

    public void RecordInsertedJobs(int insertedCount)
    {
        EnsureRunning();

        if (insertedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(insertedCount), "Inserted record count cannot be negative.");
        }

        RecordsInserted += insertedCount;
        Touch();
    }

    public void RecordHiddenJobs(int hiddenCount)
    {
        EnsureRunning();

        if (hiddenCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hiddenCount), "Hidden record count cannot be negative.");
        }

        if (RecordsHidden + hiddenCount > RecordsInserted)
        {
            throw new InvalidOperationException("Hidden record count cannot exceed inserted record count.");
        }

        RecordsHidden += hiddenCount;
        Touch();
    }

    public void RecordFailedItems(int failedCount)
    {
        EnsureRunning();

        if (failedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(failedCount), "Failed record count cannot be negative.");
        }

        RecordsFailed += failedCount;
        Touch();
    }

    public void Complete(bool partial = false)
    {
        EnsureRunning();

        Status = partial ? JobRefreshRunStatus.PartiallyCompleted : JobRefreshRunStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Fail(string failureMessage)
    {
        EnsureRunning();

        if (string.IsNullOrWhiteSpace(failureMessage))
        {
            throw new ArgumentException("Failure message is required.", nameof(failureMessage));
        }

        Status = JobRefreshRunStatus.Failed;
        FailureMessage = failureMessage.Trim();
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    private void EnsureRunning()
    {
        if (IsTerminal)
        {
            throw new InvalidOperationException("Job refresh run is already complete.");
        }
    }
}
