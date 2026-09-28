using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Request to resolve an instrument's OpenFIGI identifiers.
/// Must specify exactly ONE of: Figi OR (Ticker + MicCode).
/// </summary>
public sealed record InstrumentLookupRequest
{
    /// <summary>
    /// A known FIGI identifier to resolve/validate.
    /// When provided, Ticker and MicCode must be null.
    /// </summary>
    public string? Figi { get; init; }

    /// <summary>
    /// A ticker symbol to look up by exchange.
    /// When provided, MicCode must also be provided and Figi must be null.
    /// </summary>
    public string? Ticker { get; init; }

    /// <summary>
    /// ISO Market Identifier Code (MIC) specifying the exchange for the ticker.
    /// Required when Ticker is provided; must be null when Figi is provided.
    /// </summary>
    public string? MicCode { get; init; }

    public void Validate()
    {
        var figiSpecified = !string.IsNullOrWhiteSpace(Figi);
        var tickerSpecified = !string.IsNullOrWhiteSpace(Ticker);
        var micSpecified = !string.IsNullOrWhiteSpace(MicCode);

        var totalSpecified = (figiSpecified ? 1 : 0) + (tickerSpecified ? 1 : 0);

        if (totalSpecified == 0)
        {
            throw new InvalidOperationException("Either Figi or Ticker must be specified.");
        }

        if (totalSpecified > 1)
        {
            throw new InvalidOperationException("Only one of Figi or Ticker may be specified; not both.");
        }

        if (figiSpecified && micSpecified)
        {
            throw new InvalidOperationException("MicCode cannot be specified when Figi is provided.");
        }

        if (tickerSpecified && !micSpecified)
        {
            throw new InvalidOperationException("MicCode is required when Ticker is specified.");
        }
    }
}

/// <summary>
/// Result of resolving an instrument's OpenFIGI identifiers.
/// Contains the canonical metadata and all available FIGI levels.
/// </summary>
public sealed record InstrumentLookupResult
{
    /// <summary>
    /// Canonical ticker from OpenFIGI (preferred), or the request ticker as fallback.
    /// </summary>
    public required string Ticker { get; init; }

    /// <summary>
    /// Canonical instrument name from OpenFIGI, or the request name as fallback.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The resolved FIGI identifiers containing venue, composite, and share-class levels.
    /// </summary>
    public required FigiIdentifiers FigiIdentifiers { get; init; }
}

/// <summary>
/// Application-level port for resolving instruments through OpenFIGI.
/// Implementations are responsible for HTTP communication and response parsing.
/// </summary>
public interface IInstrumentIdentifierResolver
{
    /// <summary>
    /// Resolves an instrument's OpenFIGI identifiers from either a known FIGI or ticker + MIC.
    /// </summary>
    /// <param name="request">Lookup request specifying either Figi or (Ticker + MicCode).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing canonical ticker/name and all FIGI levels.</returns>
    /// <exception cref="InvalidOperationException">If request format is invalid (both or neither identifier pair specified).</exception>
    /// <exception cref="InstrumentNotResolvedException">If no match found in OpenFIGI.</exception>
    /// <exception cref="AmbiguousInstrumentException">If multiple matches found (ambiguous identifier).</exception>
    /// <exception cref="InstrumentIdentifierServiceException">If OpenFIGI is unavailable or rate-limited.</exception>
    Task<InstrumentLookupResult> ResolveAsync(InstrumentLookupRequest request, CancellationToken cancellationToken);
}
