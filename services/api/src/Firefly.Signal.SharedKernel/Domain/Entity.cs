namespace Firefly.Signal.SharedKernel.Domain;

public interface IAggregateRoot;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    void MarkDeleted();
}

public abstract class Entity
{
    public long Id { get; protected set; } = SnowflakeId.GenerateId();
}

public abstract class SoftDeletableEntity : Entity, ISoftDeletable
{
    public bool IsDeleted { get; private set; }

    public virtual void MarkDeleted()
    {
        IsDeleted = true;
    }
}

public abstract class AuditableEntity : SoftDeletableEntity
{
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    public void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public override void MarkDeleted()
    {
        base.MarkDeleted();
        Touch();
    }
}
