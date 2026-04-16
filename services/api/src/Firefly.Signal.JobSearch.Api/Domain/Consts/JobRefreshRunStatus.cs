namespace Firefly.Signal.JobSearch.Domain;

public enum JobRefreshRunStatus
{
    Running = 1,
    Completed = 2,
    PartiallyCompleted = 3,
    Failed = 4
}
