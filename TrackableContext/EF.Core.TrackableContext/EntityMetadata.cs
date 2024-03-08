namespace EF.Core.TrackableContext
{
    public class EntityMetadata
    {
        public required IDictionary<string, string> Properties { get; init; }
        public required string TableName { get; init; }
        public required string SchemaName { get; init; }
        public required ICollection<string> Key { get; init; }
        public required Type Type { get; init; }
    }
}