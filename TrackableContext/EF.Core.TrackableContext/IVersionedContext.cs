using EF.Core.TrackableContext.Model;

namespace EF.Core.TrackableContext;

public interface IVersionedContext 
{
    Task<long> GetLastVersion();
    IQueryable<T> GetVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
        where TEntity : class;
    IQueryable<TEntity> GetUpdated<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>;
    IQueryable<TEntity> GetInserted<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>;
    IQueryable<T> GetDeleted<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>;
    IQueryable<T> GetUpdatedVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>;
    IQueryable<T> GetInsertedVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>;
}