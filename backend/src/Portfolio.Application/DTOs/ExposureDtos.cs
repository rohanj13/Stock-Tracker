namespace Portfolio.Application.DTOs;

/// <summary>
/// Represents the geographic (country) exposure breakdown of a portfolio or ETF.
/// </summary>
public sealed class GeographicExposureResponse
{
    public Guid SourceId { get; set; }

    public IReadOnlyCollection<GeographicExposureItem> Exposures { get; set; } = [];
}

/// <summary>
/// Represents the exposure percentage attributable to a single country.
/// </summary>
public sealed class GeographicExposureItem
{
    public string CountryCode { get; set; } = string.Empty;

    public decimal Percentage { get; set; }
}

/// <summary>
/// Represents the GICS sector exposure breakdown of a portfolio or ETF.
/// </summary>
public sealed class SectorExposureResponse
{
    public Guid SourceId { get; set; }

    public IReadOnlyCollection<SectorExposureItem> Exposures { get; set; } = [];
}

/// <summary>
/// Represents the exposure percentage attributable to a single GICS sector.
/// </summary>
public sealed class SectorExposureItem
{
    public string Sector { get; set; } = string.Empty;

    public decimal Percentage { get; set; }
}

/// <summary>
/// Represents the GICS industry exposure breakdown of a portfolio or ETF.
/// </summary>
public sealed class IndustryExposureResponse
{
    public Guid SourceId { get; set; }

    public IReadOnlyCollection<IndustryExposureItem> Exposures { get; set; } = [];
}

/// <summary>
/// Represents the exposure percentage attributable to a single GICS industry.
/// </summary>
public sealed class IndustryExposureItem
{
    public string IndustryCode { get; set; } = string.Empty;

    public decimal Percentage { get; set; }
}
