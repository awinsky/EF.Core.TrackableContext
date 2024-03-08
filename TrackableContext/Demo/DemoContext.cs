using EF.Core.TrackableContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Demo;

public class DemoContext : VersionedContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        const string connectionString = "Server=(local);Initial Catalog=TrackableDemo;Integrated Security=SSPI;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connectionString);
    }
}