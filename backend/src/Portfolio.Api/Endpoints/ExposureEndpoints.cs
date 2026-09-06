using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// Endpoints for retrieving geographic, sector, and industry exposure breakdowns
/// for portfolios and ETFs.
/// </summary>
public static class ExposureEndpoints
{
    public static void MapExposureEndpoints(this IEndpointRouteBuilder app)
    {
        var portfolioGroup = app.MapGroup("/portfolios/{id:guid}/exposure")
            .WithTags("Exposure");

        portfolioGroup.MapGet("/geographic", GetPortfolioGeographicExposure)
            .WithName("GetPortfolioGeographicExposure")
            .Produces<GeographicExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the geographic exposure breakdown of a portfolio");

        portfolioGroup.MapGet("/sector", GetPortfolioSectorExposure)
            .WithName("GetPortfolioSectorExposure")
            .Produces<SectorExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the sector exposure breakdown of a portfolio");

        portfolioGroup.MapGet("/industry", GetPortfolioIndustryExposure)
            .WithName("GetPortfolioIndustryExposure")
            .Produces<IndustryExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the industry exposure breakdown of a portfolio");

        var etfGroup = app.MapGroup("/etfs/{id:guid}/exposure")
            .WithTags("Exposure");

        etfGroup.MapGet("/geographic", GetEtfGeographicExposure)
            .WithName("GetEtfGeographicExposure")
            .Produces<GeographicExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the geographic exposure breakdown of an ETF");

        etfGroup.MapGet("/sector", GetEtfSectorExposure)
            .WithName("GetEtfSectorExposure")
            .Produces<SectorExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the sector exposure breakdown of an ETF");

        etfGroup.MapGet("/industry", GetEtfIndustryExposure)
            .WithName("GetEtfIndustryExposure")
            .Produces<IndustryExposureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get the industry exposure breakdown of an ETF");
    }

    private static async Task<IResult> GetPortfolioGeographicExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetPortfolioGeographicExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }

    private static async Task<IResult> GetPortfolioSectorExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetPortfolioSectorExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }

    private static async Task<IResult> GetPortfolioIndustryExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetPortfolioIndustryExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }

    private static async Task<IResult> GetEtfGeographicExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetEtfGeographicExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }

    private static async Task<IResult> GetEtfSectorExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetEtfSectorExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }

    private static async Task<IResult> GetEtfIndustryExposure(Guid id, IExposureAnalysisService exposureService, CancellationToken cancellationToken)
    {
        var exposure = await exposureService.GetEtfIndustryExposureAsync(id, cancellationToken);
        return exposure is not null ? Results.Ok(exposure) : Results.NotFound();
    }
}
