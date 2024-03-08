namespace EF.Core.TrackableContext.Model;

public abstract class VersionedEntity : IVersionedEntity
{

    public required string Operation
    {
        get;init;
    }

    public long EntityVersion { get; set; }
}

public abstract class VersionedEntity<T> : VersionedEntity, IVersionedEntity<T>
    where T : class
{
    public virtual T Entity { get; init; } = null!;
}