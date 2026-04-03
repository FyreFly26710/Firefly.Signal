namespace Firefly.Signal.SharedKernel.Domain;

public interface IAggregateRoot;

public abstract class Entity
{
    public long Id { get; protected set; } = SnowflakeId.GenerateId();
}

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }

    public void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkDeleted()
    {
        IsDeleted = true;
        Touch();
    }
}
