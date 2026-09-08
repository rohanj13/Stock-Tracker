using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// CRUD endpoints for managing portfolios and their positions.
/// </summary>
public static class PortfolioEndpoints
{
    public static void MapPortfolioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/portfolios")
            .WithTags("Portfolios");

        group.MapGet("/", GetAllPortfolios)
            .WithName("GetAllPortfolios")
            .Produces<IReadOnlyCollection<PortfolioDto>>(StatusCodes.Status200OK)
            .WithSummary("Get all portfolios");

        group.MapGet("/{id:guid}", GetPortfolioById)
            .WithName("GetPortfolioById")
            .Produces<PortfolioDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a portfolio by id");

        group.MapPost("/", CreatePortfolio)
            .WithName("CreatePortfolio")
            .Accepts<CreatePortfolioRequest>("application/json")
            .Produces<PortfolioDto>(StatusCodes.Status201Created)
            .WithSummary("Create a new portfolio");

        group.MapPut("/{id:guid}", UpdatePortfolio)
            .WithName("UpdatePortfolio")
            .Accepts<UpdatePortfolioRequest>("application/json")
            .Produces<PortfolioDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing portfolio");

        group.MapDelete("/{id:guid}", DeletePortfolio)
            .WithName("DeletePortfolio")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete a portfolio");

        group.MapPost("/{id:guid}/positions", AddPosition)
            .WithName("AddPortfolioPosition")
            .Accepts<AddPortfolioPositionRequest>("application/json")
            .Produces<PortfolioPositionDto>(StatusCodes.Status201Created)
            .WithSummary("Add a position to a portfolio");

        group.MapDelete("/{id:guid}/positions/{positionId:guid}", RemovePosition)
            .WithName("RemovePortfolioPosition")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Remove a position from a portfolio");
    }

    private static async Task<IResult> GetAllPortfolios(IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var portfolios = await portfolioService.GetAllAsync(cancellationToken);
        return Results.Ok(portfolios);
    }

    private static async Task<IResult> GetPortfolioById(Guid id, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioService.GetByIdAsync(id, cancellationToken);
        return portfolio is not null ? Results.Ok(portfolio) : Results.NotFound();
    }

    private static async Task<IResult> CreatePortfolio(CreatePortfolioRequest request, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioService.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/portfolios/{portfolio.Id}", portfolio);
    }

    private static async Task<IResult> UpdatePortfolio(Guid id, UpdatePortfolioRequest request, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioService.UpdateAsync(id, request, cancellationToken);
        return portfolio is not null ? Results.Ok(portfolio) : Results.NotFound();
    }

    private static async Task<IResult> DeletePortfolio(Guid id, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var deleted = await portfolioService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddPosition(Guid id, AddPortfolioPositionRequest request, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var position = await portfolioService.AddPositionAsync(id, request, cancellationToken);
        return Results.Created($"/api/portfolios/{id}/positions/{position.Id}", position);
    }

    private static async Task<IResult> RemovePosition(Guid id, Guid positionId, IPortfolioService portfolioService, CancellationToken cancellationToken)
    {
        var removed = await portfolioService.RemovePositionAsync(id, positionId, cancellationToken);
        return removed ? Results.NoContent() : Results.NotFound();
    }
}
