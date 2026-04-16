using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record UpsertUserProfileCommand(
    long UserId,
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GithubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string? PreferencesJson) : IRequest<UserProfileUpsertResult?>;
