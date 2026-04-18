using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Queries;

public sealed class UserProfileQueries(JobSearchDbContext dbContext) : IUserProfileQueries
{
    public async Task<UserProfileResponse?> GetByUserAccountIdAsync(long userAccountId, CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserAccountId == userAccountId, cancellationToken);

        return profile is null ? null : new UserProfileResponse(
            profile.Id,
            profile.UserAccountId,
            profile.FullName,
            profile.PreferredTitle,
            profile.PrimaryLocationPostcode,
            profile.LinkedInUrl,
            profile.GitHubUrl,
            profile.PortfolioUrl,
            profile.Summary,
            profile.SkillsText,
            profile.ExperienceText,
            profile.PreferencesText,
            profile.CreatedAtUtc,
            profile.UpdatedAtUtc);
    }
}
