namespace Portfolio.Application.DTOs;

/// <summary>
/// Request to create a new stock.
/// Lookup is performed by either:
/// - Figi (known OpenFIGI identifier), or
/// - Ticker + MicCode (will be resolved through OpenFIGI)
/// </summary>
public sealed record CreateStockRequest
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
    /// Fallback stock name (will be overridden by OpenFIGI canonical name if available).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "GB").
    /// </summary>
    public required string Country { get; init; }

    /// <summary>
    /// GICS sector classification.
    /// </summary>
    public required string Sector { get; init; }

    /// <summary>
    /// 8-digit GICS industry code.
    /// </summary>
    public required string IndustryCode { get; init; }
}

/// <summary>
/// Request to update an existing stock.
/// FIGI identifiers cannot be changed; update any other field.
/// </summary>
public sealed record UpdateStockRequest
{
    public string? Name { get; init; }
    public string? Sector { get; init; }
    public string? IndustryCode { get; init; }
}

/// <summary>
/// Stock data transfer object with FIGI identifiers.
/// </summary>
public sealed record StockDto
{
    public required Guid Id { get; init; }

    public required string Symbol { get; init; }

    public required string Name { get; init; }

    public required string Country { get; init; }

    public required string Sector { get; init; }

    public required string IndustryCode { get; init; }

    /// <summary>
    /// Venue-level FIGI, uniquely identifying this stock on its exchange.
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
    /// Optional MIC code from the original lookup that resolved this stock's FIGI.
    /// </summary>
    public string? SourceMicCode { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
