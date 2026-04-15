using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Services;

public sealed class UserProfileQueries(IdentityDbContext dbContext) : IUserProfileQueries
{
    public async Task<UserProfileResponse?> GetCurrentAsync(long userId, CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(existingProfile => existingProfile.UserAccountId == userId, cancellationToken);

        return profile is null ? null : UserProfileResponseMappers.ToUserProfileResponse(profile);
    }
}
