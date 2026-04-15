using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.Identity.Contracts.Requests;

public sealed class UploadUserDocumentRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsDefault { get; set; }
    public IFormFile? File { get; set; }
}
