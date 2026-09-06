using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public sealed class Bond : Security
{
    public Bond(string symbol, string name, CountryCode country)
        : base(symbol, name, country, SecurityType.Bond)
    {
    }
}