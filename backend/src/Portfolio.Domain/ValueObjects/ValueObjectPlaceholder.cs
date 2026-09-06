namespace Portfolio.Domain.ValueObjects;

/// <summary>
/// Placeholder for domain value objects.
/// Value objects represent immutable domain concepts without identity.
/// Examples of value objects that will be added:
/// - Money (Amount, Currency)
/// - Percentage (Value with range validation)
/// - Ticker (Security identifier)
/// </summary>
public sealed record CountryCode
{
    public string Value { get; }

    public CountryCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length != 2 || !value.All(char.IsAsciiLetter))
        {
            throw new ArgumentException("Country code must be a two-letter ISO 3166-1 alpha-2 code.", nameof(value));
        }

        Value = value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}

public sealed record GicsIndustryCode
{
    public string Value { get; }

    public GicsIndustryCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length != 8 || !value.All(char.IsDigit))
        {
            throw new ArgumentException("GICS industry code must contain eight digits.", nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value;
}
