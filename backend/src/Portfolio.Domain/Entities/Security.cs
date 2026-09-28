using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public abstract class Security : Entity
{
    protected Security(
        string symbol,
        string name,
        CountryCode country,
        SecurityType type,
        FigiIdentifiers? figiIdentifiers = null,
        string? sourceMicCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Symbol = symbol.ToUpperInvariant();
        _name = name;
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Type = type;
        FigiIdentifiers = figiIdentifiers;
        SourceMicCode = sourceMicCode;
    }

    public string Symbol { get; }

    private string _name;
    public string Name => _name;

    public CountryCode Country { get; }

    public SecurityType Type { get; }

    /// <summary>
    /// Immutable OpenFIGI identifiers for the security.
    /// Required for stocks and ETFs; optional for bonds and cash (out of scope for initial integration).
    /// </summary>
    public FigiIdentifiers? FigiIdentifiers { get; }

    /// <summary>
    /// Optional ISO MIC code from the original lookup that resolved this security's FIGI.
    /// Helps track the resolution source for audit/debugging.
    /// </summary>
    public string? SourceMicCode { get; }

    /// <summary>
    /// Updates the security name. FIGI identifiers remain immutable.
    /// </summary>
    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
        Touch();
    }
}