using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Api.Apis;

internal static class JobApplicationApiMappers
{
    public static bool TryParseStatus(string value, out JobApplicationStatus status)
        => Enum.TryParse(value, ignoreCase: true, out status);
}
