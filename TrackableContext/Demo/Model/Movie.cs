using EF.Core.TrackableContext.Model;

namespace Demo.Model;

public interface IMovie
{
    int Id { get; set; }
} 

public class Movie : IMovie
{
    public int Id { get; set; }
    public required string Title { get; set; }
}

public class MovieVersioned : VersionedEntity<Movie>, IMovie
{
    public int Id { get; set; }
}