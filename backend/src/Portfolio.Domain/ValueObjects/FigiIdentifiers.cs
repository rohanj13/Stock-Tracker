namespace Portfolio.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing OpenFIGI identifiers for an instrument.
/// Stores the required venue FIGI and optional grouping FIGIs (composite and share-class).
/// </summary>
public sealed record FigiIdentifiers
{
    private const int FigiLength = 12;
    private const string FigiPattern = "BBG[A-Z0-9]{9}";

    /// <summary>
    /// The venue-level FIGI, uniquely identifying one listed instrument on one exchange.
    /// Always present for stocks and ETFs.
    /// </summary>
    public string Figi { get; }

    /// <summary>
    /// Optional composite FIGI, grouping all listings of an instrument in a single country/market.
    /// Null when OpenFIGI does not return a composite identifier.
    /// </summary>
    public string? CompositeFigi { get; }

    /// <summary>
    /// Optional share-class FIGI, grouping all global listings of the same instrument across all countries.
    /// Null when OpenFIGI does not return a share-class identifier.
    /// </summary>
    public string? ShareClassFigi { get; }

    public FigiIdentifiers(string figi, string? compositeFigi = null, string? shareClassFigi = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(figi);

        // Normalize to uppercase for consistency
        var normalizedFigi = figi.ToUpperInvariant();
        var normalizedComposite = compositeFigi?.ToUpperInvariant();
        var normalizedShareClass = shareClassFigi?.ToUpperInvariant();

        // Validate venue FIGI (required)
        ValidateFigi(normalizedFigi, nameof(figi));

        // Validate optional FIGIs if provided
        if (!string.IsNullOrEmpty(normalizedComposite))
        {
            ValidateFigi(normalizedComposite, nameof(compositeFigi));
        }

        if (!string.IsNullOrEmpty(normalizedShareClass))
        {
            ValidateFigi(normalizedShareClass, nameof(shareClassFigi));
        }

        Figi = normalizedFigi;
        CompositeFigi = normalizedComposite;
        ShareClassFigi = normalizedShareClass;
    }

    /// <summary>
    /// Validates FIGI format: must be exactly 12 characters, start with "BBG", followed by 9 alphanumeric characters.
    /// Does not validate check-digit (OpenFIGI already does this).
    /// </summary>
    private static void ValidateFigi(string figi, string paramName)
    {
        if (figi.Length != FigiLength)
        {
            throw new ArgumentException(
                $"FIGI must be exactly {FigiLength} characters long. Got '{figi}' with length {figi.Length}.",
                paramName);
        }

        if (!figi.StartsWith("BBG", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"FIGI must start with 'BBG'. Got '{figi}'.",
                paramName);
        }

        // Validate remaining 9 characters are alphanumeric
        var suffix = figi.Substring(3);
        if (!suffix.All(char.IsAsciiLetterOrDigit))
        {
            throw new ArgumentException(
                $"FIGI characters 4-12 must be alphanumeric. Got '{figi}'.",
                paramName);
        }
    }

    public override string ToString() => Figi;
}
