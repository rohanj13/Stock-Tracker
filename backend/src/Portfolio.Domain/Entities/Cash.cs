using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public sealed class Cash : Security
{
    public Cash(string symbol, string name, CountryCode country)
        : base(symbol, name, country, SecurityType.Cash)
    {
    }
}