using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Security base entity.
/// Uses table-per-hierarchy (TPH) inheritance strategy.
/// </summary>
public class SecurityConfiguration : IEntityTypeConfiguration<Security>
{
    public void Configure(EntityTypeBuilder<Security> builder)
    {
        builder.HasKey(s => s.Id);

        // TPH inheritance with discriminator column
        builder.HasDiscriminator<string>("SecurityType")
            .HasValue<Stock>("Stock")
            .HasValue<ExchangeTradedFund>("ExchangeTradedFund");

        // FIGI Identifiers as an owned entity (embedded value object)
        builder.OwnsOne(s => s.FigiIdentifiers, figiBuilder =>
        {
            // Map to columns with descriptive names
            figiBuilder.Property(f => f.Figi).HasColumnName("VenueFigi").IsRequired();
            figiBuilder.Property(f => f.CompositeFigi).HasColumnName("CompositeFigi").IsRequired(false);
            figiBuilder.Property(f => f.ShareClassFigi).HasColumnName("ShareClassFigi").IsRequired(false);

            // Unique index on venue FIGI (globally unique identifier)
            figiBuilder.HasIndex(f => f.Figi).IsUnique();

            // Non-unique indexes for efficient lookups by composite/share-class FIGI
            figiBuilder.HasIndex(f => f.CompositeFigi);
            figiBuilder.HasIndex(f => f.ShareClassFigi);
        });

        // Source MIC code as optional property
        builder.Property(s => s.SourceMicCode).IsRequired(false).HasMaxLength(4);

        // Common properties
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();
    }
}
