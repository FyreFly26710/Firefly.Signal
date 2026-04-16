using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Queries;

public sealed class UserQueries(IdentityDbContext dbContext) : IUserQueries
{
    public async Task<IReadOnlyList<AuthenticatedUserResponse>> ListAsync(CancellationToken cancellationToken = default)
        => await dbContext.Users
            .OrderBy(user => user.UserAccountName)
            .Select(user => IdentityResponseMappers.ToAuthenticatedUserResponse(user))
            .ToListAsync(cancellationToken);

    public async Task<AuthenticatedUserResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == id, cancellationToken);
        return user is null ? null : IdentityResponseMappers.ToAuthenticatedUserResponse(user);
    }
}
