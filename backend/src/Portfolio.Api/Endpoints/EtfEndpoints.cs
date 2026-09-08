using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// CRUD endpoints for managing exchange traded funds and their constituents.
/// </summary>
public static class EtfEndpoints
{
    public static void MapEtfEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/etfs")
            .WithTags("Etfs");

        group.MapGet("/", GetAllEtfs)
            .WithName("GetAllEtfs")
            .Produces<IReadOnlyCollection<EtfDto>>(StatusCodes.Status200OK)
            .WithSummary("Get all ETFs");

        group.MapGet("/{id:guid}", GetEtfById)
            .WithName("GetEtfById")
            .Produces<EtfDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get an ETF by id");

        group.MapPost("/", CreateEtf)
            .WithName("CreateEtf")
            .Accepts<CreateEtfRequest>("application/json")
            .Produces<EtfDto>(StatusCodes.Status201Created)
            .WithSummary("Create a new ETF");

        group.MapPut("/{id:guid}", UpdateEtf)
            .WithName("UpdateEtf")
            .Accepts<UpdateEtfRequest>("application/json")
            .Produces<EtfDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing ETF");

        group.MapDelete("/{id:guid}", DeleteEtf)
            .WithName("DeleteEtf")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete an ETF");

        group.MapPost("/{id:guid}/constituents", AddConstituent)
            .WithName("AddEtfConstituent")
            .Accepts<AddEtfConstituentRequest>("application/json")
            .Produces<EtfConstituentDto>(StatusCodes.Status201Created)
            .WithSummary("Add a constituent to an ETF");

        group.MapDelete("/{id:guid}/constituents/{constituentId:guid}", RemoveConstituent)
            .WithName("RemoveEtfConstituent")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Remove a constituent from an ETF");
    }

    private static async Task<IResult> GetAllEtfs(IEtfService etfService, CancellationToken cancellationToken)
    {
        var etfs = await etfService.GetAllAsync(cancellationToken);
        return Results.Ok(etfs);
    }

    private static async Task<IResult> GetEtfById(Guid id, IEtfService etfService, CancellationToken cancellationToken)
    {
        var etf = await etfService.GetByIdAsync(id, cancellationToken);
        return etf is not null ? Results.Ok(etf) : Results.NotFound();
    }

    private static async Task<IResult> CreateEtf(CreateEtfRequest request, IEtfService etfService, CancellationToken cancellationToken)
    {
        var etf = await etfService.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/etfs/{etf.Id}", etf);
    }

    private static async Task<IResult> UpdateEtf(Guid id, UpdateEtfRequest request, IEtfService etfService, CancellationToken cancellationToken)
    {
        var etf = await etfService.UpdateAsync(id, request, cancellationToken);
        return etf is not null ? Results.Ok(etf) : Results.NotFound();
    }

    private static async Task<IResult> DeleteEtf(Guid id, IEtfService etfService, CancellationToken cancellationToken)
    {
        var deleted = await etfService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddConstituent(Guid id, AddEtfConstituentRequest request, IEtfService etfService, CancellationToken cancellationToken)
    {
        var constituent = await etfService.AddConstituentAsync(id, request, cancellationToken);
        return Results.Created($"/api/etfs/{id}/constituents/{constituent.Id}", constituent);
    }

    private static async Task<IResult> RemoveConstituent(Guid id, Guid constituentId, IEtfService etfService, CancellationToken cancellationToken)
    {
        var removed = await etfService.RemoveConstituentAsync(id, constituentId, cancellationToken);
        return removed ? Results.NoContent() : Results.NotFound();
    }
}
