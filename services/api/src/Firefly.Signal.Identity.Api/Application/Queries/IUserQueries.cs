using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Queries;

public interface IUserQueries
{
    Task<IReadOnlyList<AuthenticatedUserResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<AuthenticatedUserResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
