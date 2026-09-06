using Microsoft.EntityFrameworkCore;

namespace Portfolio.Infrastructure.Persistence;

/// <summary>
/// Application database context for Portfolio domain.
/// Configured for PostgreSQL using Npgsql.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration for domain entities will be added here as the domain model evolves.
        // Each entity should have its own EntityTypeConfiguration in the Configurations folder.
    }
}
