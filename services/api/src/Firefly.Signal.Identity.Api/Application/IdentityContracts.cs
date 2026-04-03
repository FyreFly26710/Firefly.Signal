namespace Firefly.Signal.Identity.Application;

public sealed record RegisterUserRequest(string UserAccount, string Password, string? Email, string? DisplayName);

public sealed record LoginUserRequest(string UserAccount, string Password);

public sealed record CreateUserRequest(string UserAccount, string Password, string? Email, string? DisplayName, string Role);

public sealed record UpdateUserRequest(string? Email, string? DisplayName, string? Role);

public sealed record AuthenticatedUserResponse(long UserId, string UserAccount, string? DisplayName, string? Email, string Role);

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc, AuthenticatedUserResponse User);
