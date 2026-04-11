namespace Firefly.Signal.JobSearch.Domain;

public enum AiAnalysisRunStatus
{
    Pending = 1,
    Running = 2,
    Completed = 3,
    PartiallyCompleted = 4,
    Failed = 5
}
