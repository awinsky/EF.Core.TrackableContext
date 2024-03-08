using Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Configuration;

public class MovieConfiguration : BaseMovieConfiguration<Movie>
{
    public override void Configure(EntityTypeBuilder<Movie> builder)
    {
        base.Configure(builder);
        builder.ToTable("Movies");
       
        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
    }
}