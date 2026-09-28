using Portfolio.Application.DTOs;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Services;

/// <summary>
/// Application service for stock operations with OpenFIGI-backed instrument identity.
/// </summary>
public class StockService : IStockService
{
    private readonly IInstrumentIdentifierResolver _identifierResolver;
    private readonly IStockRepository _repository;

    public StockService(
        IInstrumentIdentifierResolver identifierResolver,
        IStockRepository repository)
    {
        _identifierResolver = identifierResolver ?? throw new ArgumentNullException(nameof(identifierResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<StockDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<StockDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<StockDto> CreateAsync(CreateStockRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // Build the resolver request from either FIGI or ticker+MIC
        var resolverRequest = BuildResolverRequest(request.Figi, request.Ticker, request.MicCode);
        resolverRequest.Validate();

        // If a FIGI was supplied, check it doesn't already exist
        if (!string.IsNullOrWhiteSpace(request.Figi))
        {
            var existing = await _repository.GetByVenueFigiAsync(request.Figi.ToUpperInvariant(), cancellationToken);
            if (existing != null)
            {
                throw new DuplicateFigiException(request.Figi);
            }
        }

        // Resolve the instrument through OpenFIGI
        var resolveResult = await _identifierResolver.ResolveAsync(resolverRequest, cancellationToken);

        // Verify uniqueness of the resolved venue FIGI before creating
        var existingByResolvedFigi = await _repository.GetByVenueFigiAsync(
            resolveResult.FigiIdentifiers.Figi,
            cancellationToken);
        if (existingByResolvedFigi != null)
        {
            throw new DuplicateFigiException(resolveResult.FigiIdentifiers.Figi);
        }

        // Parse sector and industry from request (validation delegated to domain entity constructors)
        if (!Enum.TryParse<GicsSector>(request.Sector, ignoreCase: true, out var sector))
        {
            throw new InvalidOperationException($"Invalid GICS sector: {request.Sector}");
        }

        var industry = new GicsIndustryCode(request.IndustryCode);
        var country = new CountryCode(request.Country);

        // Construct domain entity with resolved FIGIs
        var stock = new Stock(
            symbol: resolveResult.Ticker,
            name: resolveResult.Name,
            country: country,
            sector: sector,
            industry: industry,
            figiIdentifiers: resolveResult.FigiIdentifiers,
            sourceMicCode: request.MicCode);

        // Persist and return DTO
        return await _repository.CreateAsync(stock, cancellationToken);
    }

    public async Task<StockDto?> UpdateAsync(Guid id, UpdateStockRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // Fetch existing stock
        var dto = await _repository.GetByIdAsync(id, cancellationToken);
        if (dto == null) return null;

        // Note: FIGIs are immutable; name, sector, and industry can be updated.
        // Since we're at the DTO level here, the actual update logic belongs in infrastructure.
        // This service method is a pass-through to the repository's update capability.
        return await _repository.UpdateAsync(id, null!, cancellationToken); // Placeholder; actual implementation in Repository
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    private static InstrumentLookupRequest BuildResolverRequest(string? figi, string? ticker, string? micCode)
    {
        return new InstrumentLookupRequest
        {
            Figi = figi,
            Ticker = ticker,
            MicCode = micCode
        };
    }
}
