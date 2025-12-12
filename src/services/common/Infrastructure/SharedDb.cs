using Microsoft.EntityFrameworkCore;

namespace Ev.Common.Infrastructure;

/// <summary>
/// Base DbContext that other service contexts inherit from.
/// It provides a generic Set<T>() method that can be used by derived contexts.
/// </summary>
public abstract class AppDbContext : DbContext
{
    protected AppDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Gives derived contexts access to the underlying DbSet for a given entity type.
    /// </summary>
    protected DbSet<T> SetEntity<T>() where T : class => Set<T>();
}
