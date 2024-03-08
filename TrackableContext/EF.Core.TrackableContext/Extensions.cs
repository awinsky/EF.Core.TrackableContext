using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EF.Core.TrackableContext.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Core.TrackableContext;

public static class Extensions
{
    public static void HasVersion<TEntity, TVersioned>(this EntityTypeBuilder<TVersioned> builder,
        Expression<Func<TEntity, object?>> foreignKey)
        where TEntity : class
        where TVersioned : class, IVersionedEntity<TEntity>
    {
        builder.Property(p => p.Operation).HasColumnName("SYS_CHANGE_OPERATION");
        builder.Property(p => p.EntityVersion).HasColumnName("SYS_CHANGE_VERSION");
        builder.HasOne(x => x.Entity).WithOne()
            .HasForeignKey(foreignKey);

        var entityType = builder.Metadata.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Metadata for {typeof(TEntity).FullName} not found");
        }

        builder.ToSqlQuery(GetTrackableSql(GetMetadata(entityType)));
    }


    private static EntityMetadata GetMetadata(IMutableEntityType entityType)
    {
        IDictionary<string, string> properties = entityType.GetProperties()
            .Where(x => x.PropertyInfo != null).ToDictionary(k => k.PropertyInfo!.Name, GetPropertyColumnName);
        var table = entityType.GetTableName() ?? throw new InvalidOperationException("Table name cannot be defined");
        var schema = entityType.GetSchema() ?? entityType.GetDefaultSchema() ?? "dbo";
        var primaryKey = entityType.FindPrimaryKey() ??
                         throw new InvalidOperationException("Primary key cannot be defined");
        ;

        return new EntityMetadata
        {
            Properties = properties,
            TableName = table,
            SchemaName = schema,
            Key = primaryKey.Properties.Select(GetPropertyColumnName).ToList(),
            Type = entityType.ClrType
        };
    }

    private static string GetTrackableSql(EntityMetadata metadata)
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
        builder.Append("], 0) AS ch");
        return builder.ToString();
    }

    private static string GetPropertyColumnName(IMutableProperty property)
    {
        var storeObjectId = StoreObjectIdentifier.Create(property.DeclaringType, StoreObjectType.Table);

        return property.GetColumnName(storeObjectId.GetValueOrDefault()) ??
               throw new InvalidOperationException("Column name cannot be defined");
    }
}