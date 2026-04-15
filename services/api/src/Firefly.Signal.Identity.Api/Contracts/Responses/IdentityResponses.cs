namespace Firefly.Signal.Identity.Contracts.Responses;

public sealed record AuthenticatedUserResponse(long UserId, string UserAccount, string? DisplayName, string? Email, string Role);

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc, AuthenticatedUserResponse User);
