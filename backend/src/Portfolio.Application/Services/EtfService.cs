using Portfolio.Application.DTOs;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Services;

/// <summary>
/// Application service for ETF operations with OpenFIGI-backed instrument identity.
/// </summary>
public class EtfService : IEtfService
{
    private readonly IInstrumentIdentifierResolver _identifierResolver;
    private readonly IEtfRepository _repository;

    public EtfService(
        IInstrumentIdentifierResolver identifierResolver,
        IEtfRepository repository)
    {
        _identifierResolver = identifierResolver ?? throw new ArgumentNullException(nameof(identifierResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<EtfDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<EtfDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<EtfDto> CreateAsync(CreateEtfRequest request, CancellationToken cancellationToken)
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

        var country = new CountryCode(request.DomicileCountry);

        // Construct domain entity with resolved FIGIs
        var etf = new ExchangeTradedFund(
            symbol: resolveResult.Ticker,
            name: resolveResult.Name,
            domicileCountry: country,
            figiIdentifiers: resolveResult.FigiIdentifiers,
            sourceMicCode: request.MicCode);

        // Persist and return DTO
        return await _repository.CreateAsync(etf, cancellationToken);
    }

    public async Task<EtfDto?> UpdateAsync(Guid id, UpdateEtfRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // Fetch existing ETF
        var dto = await _repository.GetByIdAsync(id, cancellationToken);
        if (dto == null) return null;

        // Note: FIGIs are immutable; name can be updated.
        // Since we're at the DTO level here, the actual update logic belongs in infrastructure.
        // This service method is a pass-through to the repository's update capability.
        return await _repository.UpdateAsync(id, null!, cancellationToken); // Placeholder; actual implementation in Repository
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<EtfConstituentDto> AddConstituentAsync(Guid id, AddEtfConstituentRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await _repository.AddConstituentAsync(id, request.SecurityId, request.Weight, cancellationToken);
    }

    public async Task<bool> RemoveConstituentAsync(Guid id, Guid constituentId, CancellationToken cancellationToken)
    {
        return await _repository.RemoveConstituentAsync(id, constituentId, cancellationToken);
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
