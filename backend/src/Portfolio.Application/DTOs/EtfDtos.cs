namespace Portfolio.Application.DTOs;

/// <summary>
/// Request to create a new ETF.
/// Lookup is performed by either:
/// - Figi (known OpenFIGI identifier), or
/// - Ticker + MicCode (will be resolved through OpenFIGI)
/// </summary>
public sealed record CreateEtfRequest
{
    /// <summary>
    /// Optional known FIGI. If provided, Ticker and MicCode must be null.
    /// </summary>
    public string? Figi { get; init; }

    /// <summary>
    /// Optional ticker symbol. If provided, MicCode must also be provided and Figi must be null.
    /// </summary>
    public string? Ticker { get; init; }

    /// <summary>
    /// Optional ISO MIC code. Required if Ticker is provided; must be null if Figi is provided.
    /// </summary>
    public string? MicCode { get; init; }

    /// <summary>
    /// Fallback ETF name (will be overridden by OpenFIGI canonical name if available).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// ISO 3166-1 alpha-2 country code for the ETF's domicile (e.g., "US", "GB").
    /// </summary>
    public required string DomicileCountry { get; init; }
}

/// <summary>
/// Request to update an existing ETF.
/// FIGI identifiers cannot be changed; update any other field.
/// </summary>
public sealed record UpdateEtfRequest
{
    public string? Name { get; init; }
}

/// <summary>
/// Exchange Traded Fund data transfer object with FIGI identifiers.
/// </summary>
public sealed record EtfDto
{
    public required Guid Id { get; init; }

    public required string Symbol { get; init; }

    public required string Name { get; init; }

    public required string DomicileCountry { get; init; }

    /// <summary>
    /// Venue-level FIGI, uniquely identifying this ETF on its exchange.
    /// </summary>
    public required string Figi { get; init; }

    /// <summary>
    /// Optional composite FIGI, grouping all listings in the same country/market.
    /// </summary>
    public string? CompositeFigi { get; init; }

    /// <summary>
    /// Optional share-class FIGI, grouping all global listings of the same instrument.
    /// </summary>
    public string? ShareClassFigi { get; init; }

    /// <summary>
    /// Optional MIC code from the original lookup that resolved this ETF's FIGI.
    /// </summary>
    public string? SourceMicCode { get; init; }

    public IReadOnlyCollection<EtfConstituentDto> Constituents { get; init; } = [];

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// Represents a single constituent holding within an ETF.
/// </summary>
public sealed record EtfConstituentDto
{
    public required Guid Id { get; init; }

    public required Guid SecurityId { get; init; }

    public required decimal Weight { get; init; }
}

/// <summary>
/// Request payload for adding a constituent to an ETF.
/// </summary>
public sealed record AddEtfConstituentRequest
{
    public required Guid SecurityId { get; init; }

    public required decimal Weight { get; init; }
}
