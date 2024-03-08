using Demo.Model;
using EF.Core.TrackableContext;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Configuration;

public class MovieVersionedConfiguration : BaseMovieConfiguration<MovieVersioned>
{
    public override void Configure(EntityTypeBuilder<MovieVersioned> builder)
    {
        base.Configure(builder);
        builder.HasVersion<Movie, MovieVersioned>(e =>
            new { e.Id });
    }

}