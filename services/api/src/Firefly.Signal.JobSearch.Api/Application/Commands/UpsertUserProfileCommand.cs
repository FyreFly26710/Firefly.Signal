using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record UpsertUserProfileCommand(
    long UserAccountId,
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string? PreferencesText) : IRequest<UserProfileResponse>;
