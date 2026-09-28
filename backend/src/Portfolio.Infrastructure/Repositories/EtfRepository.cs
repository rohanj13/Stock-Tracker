using Microsoft.EntityFrameworkCore;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IEtfRepository.
/// </summary>
public class EtfRepository : IEtfRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EtfRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyCollection<EtfDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExchangeTradedFunds
            .Include(e => e.Constituents)
            .Select(e => MapToDto(e))
            .ToListAsync(cancellationToken);
    }

    public async Task<EtfDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var etf = await _dbContext.ExchangeTradedFunds
            .Include(e => e.Constituents)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return etf == null ? null : MapToDto(etf);
    }

    public async Task<ExchangeTradedFund?> GetByVenueFigiAsync(string venueFigi, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExchangeTradedFunds
            .FirstOrDefaultAsync(e => e.FigiIdentifiers!.Figi == venueFigi, cancellationToken);
    }

    public async Task<EtfDto> CreateAsync(ExchangeTradedFund etf, CancellationToken cancellationToken = default)
    {
        if (etf == null) throw new ArgumentNullException(nameof(etf));

        _dbContext.ExchangeTradedFunds.Add(etf);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(etf);
    }

    public async Task<EtfDto?> UpdateAsync(Guid id, ExchangeTradedFund etf, CancellationToken cancellationToken = default)
    {
        if (etf == null) throw new ArgumentNullException(nameof(etf));

        var existing = await _dbContext.ExchangeTradedFunds
            .Include(e => e.Constituents)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existing == null) return null;

        // Update only mutable properties (name)
        // FIGI identifiers are immutable and cannot be changed
        existing.UpdateName(etf.Name);

        _dbContext.ExchangeTradedFunds.Update(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(existing);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var etf = await _dbContext.ExchangeTradedFunds
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (etf == null) return false;

        _dbContext.ExchangeTradedFunds.Remove(etf);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<EtfConstituentDto> AddConstituentAsync(Guid etfId, Guid securityId, decimal weight, CancellationToken cancellationToken = default)
    {
        var etf = await _dbContext.ExchangeTradedFunds
            .Include(e => e.Constituents)
            .FirstOrDefaultAsync(e => e.Id == etfId, cancellationToken);

        if (etf == null)
            throw new InvalidOperationException($"ETF with ID {etfId} not found");

        // Use domain method to add constituent
        var constituent = etf.AddConstituent(securityId, weight);

        _dbContext.ExchangeTradedFunds.Update(etf);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapConstituentToDto(constituent);
    }

    public async Task<bool> RemoveConstituentAsync(Guid etfId, Guid constituentId, CancellationToken cancellationToken = default)
    {
        var etf = await _dbContext.ExchangeTradedFunds
            .Include(e => e.Constituents)
            .FirstOrDefaultAsync(e => e.Id == etfId, cancellationToken);

        if (etf == null) return false;

        var constituent = etf.Constituents.FirstOrDefault(c => c.Id == constituentId);
        if (constituent == null) return false;

        etf.RemoveConstituent(constituent.Id);

        _dbContext.ExchangeTradedFunds.Update(etf);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static EtfDto MapToDto(ExchangeTradedFund etf)
    {
        return new EtfDto
        {
            Id = etf.Id,
            Symbol = etf.Symbol,
            Name = etf.Name,
            DomicileCountry = etf.DomicileCountry.Value,
            Figi = etf.FigiIdentifiers!.Figi,
            CompositeFigi = etf.FigiIdentifiers.CompositeFigi,
            ShareClassFigi = etf.FigiIdentifiers.ShareClassFigi,
            SourceMicCode = etf.SourceMicCode,
            Constituents = etf.Constituents.Select(MapConstituentToDto).ToList(),
            CreatedAt = etf.CreatedAt.DateTime,
            UpdatedAt = etf.UpdatedAt.DateTime
        };
    }

    private static EtfConstituentDto MapConstituentToDto(EtfConstituent constituent)
    {
        return new EtfConstituentDto
        {
            Id = constituent.Id,
            SecurityId = constituent.SecurityId,
            Weight = constituent.Weight
        };
    }
}
