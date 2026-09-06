using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// CRUD endpoints for managing stock securities.
/// </summary>
public static class StockEndpoints
{
    public static void MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/stocks")
            .WithTags("Stocks");

        group.MapGet("/", GetAllStocks)
            .WithName("GetAllStocks")
            .Produces<IReadOnlyCollection<StockDto>>(StatusCodes.Status200OK)
            .WithSummary("Get all stocks");

        group.MapGet("/{id:guid}", GetStockById)
            .WithName("GetStockById")
            .Produces<StockDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a stock by id");

        group.MapPost("/", CreateStock)
            .WithName("CreateStock")
            .Accepts<CreateStockRequest>("application/json")
            .Produces<StockDto>(StatusCodes.Status201Created)
            .WithSummary("Create a new stock");

        group.MapPut("/{id:guid}", UpdateStock)
            .WithName("UpdateStock")
            .Accepts<UpdateStockRequest>("application/json")
            .Produces<StockDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing stock");

        group.MapDelete("/{id:guid}", DeleteStock)
            .WithName("DeleteStock")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete a stock");
    }

    private static async Task<IResult> GetAllStocks(IStockService stockService, CancellationToken cancellationToken)
    {
        var stocks = await stockService.GetAllAsync(cancellationToken);
        return Results.Ok(stocks);
    }

    private static async Task<IResult> GetStockById(Guid id, IStockService stockService, CancellationToken cancellationToken)
    {
        var stock = await stockService.GetByIdAsync(id, cancellationToken);
        return stock is not null ? Results.Ok(stock) : Results.NotFound();
    }

    private static async Task<IResult> CreateStock(CreateStockRequest request, IStockService stockService, CancellationToken cancellationToken)
    {
        var stock = await stockService.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/stocks/{stock.Id}", stock);
    }

    private static async Task<IResult> UpdateStock(Guid id, UpdateStockRequest request, IStockService stockService, CancellationToken cancellationToken)
    {
        var stock = await stockService.UpdateAsync(id, request, cancellationToken);
        return stock is not null ? Results.Ok(stock) : Results.NotFound();
    }

    private static async Task<IResult> DeleteStock(Guid id, IStockService stockService, CancellationToken cancellationToken)
    {
        var deleted = await stockService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
