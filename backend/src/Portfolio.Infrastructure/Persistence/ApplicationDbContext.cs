using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence.Configurations;

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

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<ExchangeTradedFund> ExchangeTradedFunds { get; set; }
    public DbSet<EtfConstituent> EtfConstituents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity type configurations
        modelBuilder.ApplyConfiguration(new SecurityConfiguration());
        modelBuilder.ApplyConfiguration(new StockConfiguration());
        modelBuilder.ApplyConfiguration(new ExchangeTradedFundConfiguration());
        modelBuilder.ApplyConfiguration(new EtfConstituentConfiguration());
    }
}
