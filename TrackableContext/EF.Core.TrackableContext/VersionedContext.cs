using System.Text;
using EF.Core.TrackableContext.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EF.Core.TrackableContext;

public abstract class VersionedContext : DbContext, IVersionedContext
{
    private readonly Dictionary<Type, EntityMetadata> _entityMetadata = new();

    private readonly Dictionary<Type, string> _versionedSql = new();

    protected VersionedContext()
    {
    }

    protected VersionedContext(DbContextOptions dbContextOptions) :
        base(dbContextOptions)
    {
    }

    public virtual async Task<long> GetLastVersion()
    {
        await using var connection = new SqlConnection(Database.GetDbConnection().ConnectionString);
        await using var command = new SqlCommand("SELECT CHANGE_TRACKING_CURRENT_VERSION()", connection);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value) return 0;

        return (long)result;
    }


    public IQueryable<TEntity> GetUpdated<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
    {
        var sql = GetVersionedSql(typeof(TEntity));
        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        return Set<T>().FromSqlRaw(sql, versionParameter).Where(x => x.Operation == "U")
            .Include(x => x.Entity)
            .AsNoTracking()
            .Select(s => s.Entity);
    }

    public IQueryable<TEntity> GetInserted<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
    {
        var sql = GetVersionedSql(typeof(TEntity));
        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        return Set<T>().FromSqlRaw(sql, versionParameter).Where(x => x.Operation == "I").Include(x => x.Entity)
            .AsNoTracking()
            .Select(s => s.Entity);
    }

    public IQueryable<T> GetUpdatedVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
    {
        var sql = GetVersionedSql(typeof(TEntity));
        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        return Set<T>().FromSqlRaw(sql, versionParameter).AsNoTracking().Where(x => x.Operation == "U").Include(x => x.Entity)
            .AsNoTracking();
    }

    public IQueryable<T> GetInsertedVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
    {
        var sql = GetVersionedSql(typeof(TEntity));
        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        return Set<T>().FromSqlRaw(sql, versionParameter).AsNoTracking().Where(x => x.Operation == "I").Include(x => x.Entity)
            .AsNoTracking();
    }


    public IQueryable<T> GetDeleted<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
    {
        var sql = GetVersionedSql(typeof(TEntity));
        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        return Set<T>().FromSqlRaw(sql, versionParameter).AsNoTracking().Where(x => x.Operation == "D")
            .AsNoTracking();
    }


    public IQueryable<T> GetVersioned<T, TEntity>(long version) where T : class, IVersionedEntity<TEntity>
        where TEntity : class
    {
        var entityType = Model.GetEntityTypes().FirstOrDefault(x => x.ClrType == typeof(TEntity));
        if (entityType == null) throw new ArgumentException($"Type {typeof(T).Name} is not defined as entity type");

        var versionParameter = new SqlParameter("@last_synchronization_version", version);
        var sql = GetSqlVersioned(GetEntityMetaData(entityType));

        var result = Set<T>().FromSqlRaw(sql, versionParameter).AsNoTracking();
        return result.Include(x => x.Entity).AsNoTracking();
    }


    private EntityMetadata GetEntityMetaData(IEntityType entityType)
    {
        if (_entityMetadata.TryGetValue(entityType.ClrType, out var data)) return data;
        var primaryKey = entityType.FindPrimaryKey();

        if (primaryKey == null) throw new InvalidOperationException("No primary key is defined");

        var metadata = new EntityMetadata
        {
            Properties = entityType.GetProperties()
                .Where(x => x.PropertyInfo != null)
                .ToDictionary(k => k.PropertyInfo!.Name, GetColumnName),

            TableName =
                entityType.GetTableName() ?? throw new InvalidOperationException("Table name cannot be defined"),

            SchemaName = entityType.GetSchema() ?? entityType.GetDefaultSchema() ?? "dbo",
            Key = primaryKey.Properties.Select(GetColumnName).ToList(),

            Type = entityType.ClrType
        };
        _entityMetadata.Add(entityType.ClrType, metadata);

        return _entityMetadata[entityType.ClrType];
    }

    private static string GetColumnName(IProperty property)
    {
        var storeObjectId = StoreObjectIdentifier.Create(property.DeclaringType, StoreObjectType.Table);

        return property.GetColumnName(storeObjectId.GetValueOrDefault()) ??
               throw new InvalidOperationException("Column name cannot be defined");
    }

    private string GetVersionedSql(Type type)
    {
        var entityType = Model.GetEntityTypes().FirstOrDefault(x => x.ClrType == type);
        if (entityType == null) throw new ArgumentException($"Type {type.Name} is not defined as entity type");

        // ReSharper disable once InvertIf
        if (!_versionedSql.ContainsKey(type))
        {
            var sql = GetSqlVersioned(GetEntityMetaData(entityType));
            _versionedSql.Add(type, sql);
        }

        return _versionedSql[type];
    }

    private static string GetSqlVersioned(EntityMetadata metadata)
    {
        var builder = new StringBuilder();
        builder.Append("SELECT ");
        foreach (var key in metadata.Key)
        {
            builder.Append("ch.[");

            builder.Append(key);
            builder.Append("], ");
        }

        builder.Append("ch.SYS_CHANGE_OPERATION, ");
        builder.Append("ch.SYS_CHANGE_VERSION ");
        builder.Append(" FROM CHANGETABLE(CHANGES ");
        builder.Append('[');
        builder.Append(metadata.SchemaName);
        builder.Append("].[");
        builder.Append(metadata.TableName);
        builder.Append("], @last_synchronization_version) AS ch");
        return builder.ToString();
    }
}