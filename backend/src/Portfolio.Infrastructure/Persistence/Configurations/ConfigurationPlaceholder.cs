namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Placeholder for Entity Framework Core configurations.
/// Each entity configuration should be in its own class implementing IEntityTypeConfiguration{T}.
/// 
/// Example configuration pattern:
/// public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
/// {
///     public void Configure(EntityTypeBuilder<Portfolio> builder)
///     {
///         builder.HasKey(p => p.Id);
///         builder.Property(p => p.Name).IsRequired().HasMaxLength(256);
///         // ... additional configuration
///     }
/// }
/// 
/// Configurations are registered in ApplicationDbContext.OnModelCreating() method.
/// </summary>
public static class ConfigurationPlaceholder
{
}
