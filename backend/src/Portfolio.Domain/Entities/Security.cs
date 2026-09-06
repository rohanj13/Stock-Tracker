using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public abstract class Security : Entity
{
    protected Security(string symbol, string name, CountryCode country, SecurityType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Symbol = symbol.ToUpperInvariant();
        Name = name;
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Type = type;
    }

    public string Symbol { get; }

    public string Name { get; }

    public CountryCode Country { get; }

    public SecurityType Type { get; }
}