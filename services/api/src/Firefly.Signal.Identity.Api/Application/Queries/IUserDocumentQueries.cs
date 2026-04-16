using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Queries;

public interface IUserDocumentQueries
{
    Task<IReadOnlyList<UserDocumentResponse>> ListAsync(long userId, CancellationToken cancellationToken = default);
    Task<UserDocumentResponse?> GetByIdAsync(long userId, long id, CancellationToken cancellationToken = default);
}
