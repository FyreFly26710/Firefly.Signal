using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Links a user-owned document to an application with explicit purpose metadata.
/// </summary>
public sealed class ApplicationDocumentLink : AuditableEntity
{
    private ApplicationDocumentLink()
    {
    }

    public long JobApplicationId { get; private set; }
    public long UserDocumentId { get; private set; }
    public ApplicationDocumentLinkType LinkType { get; private set; }

    public static ApplicationDocumentLink Create(
        long jobApplicationId,
        long userDocumentId,
        ApplicationDocumentLinkType linkType)
    {
        return new ApplicationDocumentLink
        {
            JobApplicationId = jobApplicationId,
            UserDocumentId = userDocumentId,
            LinkType = linkType
        };
    }

    public void ChangeLinkType(ApplicationDocumentLinkType linkType)
    {
        LinkType = linkType;
        Touch();
    }
}
