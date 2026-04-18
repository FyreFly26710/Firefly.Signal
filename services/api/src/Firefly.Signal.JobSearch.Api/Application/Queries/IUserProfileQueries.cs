using Firefly.Signal.JobSearch.Contracts.Responses;

namespace Firefly.Signal.JobSearch.Application.Queries;

public interface IUserProfileQueries
{
    Task<UserProfileResponse?> GetByUserAccountIdAsync(long userAccountId, CancellationToken cancellationToken = default);
}
