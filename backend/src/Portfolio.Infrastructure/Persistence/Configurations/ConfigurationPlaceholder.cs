using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public sealed class SecurityConfiguration : IEntityTypeConfiguration<Security>
{
    public void Configure(EntityTypeBuilder<Security> builder)
    {
        builder.ToTable(
            "Securities",
            tableBuilder => tableBuilder.HasCheckConstraint("CK_Securities_CountryCode_Length", "char_length(\"CountryCode\") = 2"));

        builder.HasKey(security => security.Id);

        builder.Property(security => security.Symbol)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(security => security.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(security => security.Country)
            .HasConversion(country => country.Value, value => new CountryCode(value))
            .HasColumnName("CountryCode")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        builder.Property(security => security.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasDiscriminator(security => security.Type)
            .HasValue<Stock>(SecurityType.Stock)
            .HasValue<ExchangeTradedFund>(SecurityType.ExchangeTradedFund)
            .HasValue<Bond>(SecurityType.Bond)
            .HasValue<Cash>(SecurityType.Cash);

    }
}

public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable(
            "Securities",
            tableBuilder => tableBuilder.HasCheckConstraint("CK_Securities_GicsIndustryCode_Length", "char_length(\"GicsIndustryCode\") = 8"));

        builder.Property(stock => stock.Sector)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(stock => stock.Industry)
            .HasConversion(industry => industry.Value, value => new GicsIndustryCode(value))
            .HasColumnName("GicsIndustryCode")
            .HasMaxLength(8)
            .IsFixedLength()
            .IsRequired();

    }
}
