using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public sealed class Stock : Security
{
    public Stock(string symbol, string name, CountryCode country, GicsSector sector, GicsIndustryCode industry)
        : base(symbol, name, country, SecurityType.Stock)
    {
        Sector = sector;
        Industry = industry ?? throw new ArgumentNullException(nameof(industry));
    }

    public GicsSector Sector { get; }

    public GicsIndustryCode Industry { get; }
}