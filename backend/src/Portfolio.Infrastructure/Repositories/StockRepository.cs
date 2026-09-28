using Microsoft.EntityFrameworkCore;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IStockRepository.
/// </summary>
public class StockRepository : IStockRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StockRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyCollection<StockDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stocks
            .Select(s => MapToDto(s))
            .ToListAsync(cancellationToken);
    }

    public async Task<StockDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stock = await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return stock == null ? null : MapToDto(stock);
    }

    public async Task<Stock?> GetByVenueFigiAsync(string venueFigi, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.FigiIdentifiers!.Figi == venueFigi, cancellationToken);
    }

    public async Task<StockDto> CreateAsync(Stock stock, CancellationToken cancellationToken = default)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock));

        _dbContext.Stocks.Add(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(stock);
    }

    public async Task<StockDto?> UpdateAsync(Guid id, Stock stock, CancellationToken cancellationToken = default)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock));

        var existing = await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (existing == null) return null;

        // Update only mutable properties (name, sector, industry)
        // FIGI identifiers are immutable and cannot be changed
        existing.UpdateName(stock.Name);

        _dbContext.Stocks.Update(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(existing);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stock = await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (stock == null) return false;

        _dbContext.Stocks.Remove(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static StockDto MapToDto(Stock stock)
    {
        return new StockDto
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            Name = stock.Name,
            Country = stock.Country.Value,
            Sector = stock.Sector.ToString(),
            IndustryCode = stock.Industry.Value,
            Figi = stock.FigiIdentifiers!.Figi,
            CompositeFigi = stock.FigiIdentifiers.CompositeFigi,
            ShareClassFigi = stock.FigiIdentifiers.ShareClassFigi,
            SourceMicCode = stock.SourceMicCode,
            CreatedAt = stock.CreatedAt.DateTime,
            UpdatedAt = stock.UpdatedAt.DateTime
        };
    }
}
