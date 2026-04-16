namespace Firefly.Signal.SharedKernel.Services;

public interface IIdentityService
{
    long? GetUserId();
    string? GetUserName();
    bool IsAuthenticated();
    bool IsInRole(string role);
}
