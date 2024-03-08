namespace EF.Core.TrackableContext.Model
{
    public interface IVersionedEntity
    {
        string Operation { get; init; }
        long EntityVersion { get; set; }
    }


    public interface IVersionedEntity<T> : IVersionedEntity
    {
        T Entity { get; init; }
    }

   
}