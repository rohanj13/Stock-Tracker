using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Defines operations for calculating geographic, sector, and industry exposure
/// for portfolios and ETFs.
/// </summary>
public interface IExposureAnalysisService
{
    Task<GeographicExposureResponse?> GetPortfolioGeographicExposureAsync(Guid portfolioId, CancellationToken cancellationToken = default);

    Task<SectorExposureResponse?> GetPortfolioSectorExposureAsync(Guid portfolioId, CancellationToken cancellationToken = default);

    Task<IndustryExposureResponse?> GetPortfolioIndustryExposureAsync(Guid portfolioId, CancellationToken cancellationToken = default);

    Task<GeographicExposureResponse?> GetEtfGeographicExposureAsync(Guid etfId, CancellationToken cancellationToken = default);

    Task<SectorExposureResponse?> GetEtfSectorExposureAsync(Guid etfId, CancellationToken cancellationToken = default);

    Task<IndustryExposureResponse?> GetEtfIndustryExposureAsync(Guid etfId, CancellationToken cancellationToken = default);
}
