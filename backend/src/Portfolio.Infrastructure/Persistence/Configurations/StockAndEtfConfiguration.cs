using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Stock entity.
/// </summary>
public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        // Map to specific table within TPH inheritance
        builder.ToTable("Securities");

        // Stock-specific properties
        builder.Property(s => s.Symbol).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Country).IsRequired().HasMaxLength(2);
        builder.Property(s => s.Sector).IsRequired().HasMaxLength(100);

        // Industry as an owned value object (if applicable)
        builder.OwnsOne(s => s.Industry, industryBuilder =>
        {
            industryBuilder.Property(i => i.Value).HasColumnName("GicsIndustryCode").IsRequired();
        });

        // Index on venue FIGI for efficient lookups (inherited from Security config)
        builder.HasIndex(s => s.FigiIdentifiers!.Figi).IsUnique();
    }
}

/// <summary>
/// EF Core configuration for ExchangeTradedFund entity.
/// </summary>
public class ExchangeTradedFundConfiguration : IEntityTypeConfiguration<ExchangeTradedFund>
{
    public void Configure(EntityTypeBuilder<ExchangeTradedFund> builder)
    {
        // Map to specific table within TPH inheritance
        builder.ToTable("Securities");

        // ETF-specific properties
        builder.Property(e => e.Symbol).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(500);
        builder.Property(e => e.DomicileCountry).IsRequired().HasMaxLength(2);

        // Constituents collection navigation
        builder.HasMany(e => e.Constituents)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        // Index on venue FIGI for efficient lookups (inherited from Security config)
        builder.HasIndex(e => e.FigiIdentifiers!.Figi).IsUnique();
    }
}

/// <summary>
/// EF Core configuration for EtfConstituent entity.
/// </summary>
public class EtfConstituentConfiguration : IEntityTypeConfiguration<EtfConstituent>
{
    public void Configure(EntityTypeBuilder<EtfConstituent> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.ExchangeTradedFundId).IsRequired();
        builder.Property(c => c.SecurityId).IsRequired();
        builder.Property(c => c.Weight).IsRequired().HasPrecision(5, 4); // 99.9999%

        // Unique constraint on (ExchangeTradedFundId, SecurityId) to prevent duplicate constituents
        builder.HasIndex(c => new { c.ExchangeTradedFundId, c.SecurityId }).IsUnique();
    }
}
