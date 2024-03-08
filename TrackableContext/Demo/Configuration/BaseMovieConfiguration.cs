using Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Configuration;

public abstract class BaseMovieConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IMovie
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
    }
}