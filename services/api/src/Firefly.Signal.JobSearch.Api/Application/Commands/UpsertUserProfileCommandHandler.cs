using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class UpsertUserProfileCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<UpsertUserProfileCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(UpsertUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserAccountId == request.UserAccountId, cancellationToken);

        if (profile is null)
        {
            profile = UserProfile.Create(
                request.UserAccountId,
                request.FullName,
                request.PreferredTitle,
                request.PrimaryLocationPostcode,
                request.LinkedInUrl,
                request.GitHubUrl,
                request.PortfolioUrl,
                request.Summary,
                request.SkillsText,
                request.ExperienceText,
                request.PreferencesText);

            dbContext.UserProfiles.Add(profile);
        }
        else
        {
            profile.Update(
                request.FullName,
                request.PreferredTitle,
                request.PrimaryLocationPostcode,
                request.LinkedInUrl,
                request.GitHubUrl,
                request.PortfolioUrl,
                request.Summary,
                request.SkillsText,
                request.ExperienceText,
                request.PreferencesText);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(
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
