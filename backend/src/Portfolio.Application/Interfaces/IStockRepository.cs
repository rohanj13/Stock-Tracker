using Portfolio.Application.DTOs;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Application port for stock repository operations.
/// Implementations abstract EF Core and other data access concerns.
/// </summary>
public interface IStockRepository
{
    /// <summary>
    /// Retrieves all stocks.
    /// </summary>
    Task<IReadOnlyCollection<StockDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a stock by ID.
    /// </summary>
    Task<StockDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a stock by venue FIGI.
    /// </summary>
    Task<Stock?> GetByVenueFigiAsync(string venueFigi, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and persists a new stock.
    /// </summary>
    Task<StockDto> CreateAsync(Stock stock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing stock.
    /// </summary>
    Task<StockDto?> UpdateAsync(Guid id, Stock stock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a stock by ID.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Application port for ETF repository operations.
/// Implementations abstract EF Core and other data access concerns.
/// </summary>
public interface IEtfRepository
{
    /// <summary>
    /// Retrieves all ETFs.
    /// </summary>
    Task<IReadOnlyCollection<EtfDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an ETF by ID.
    /// </summary>
    Task<EtfDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an ETF by venue FIGI.
    /// </summary>
    Task<ExchangeTradedFund?> GetByVenueFigiAsync(string venueFigi, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and persists a new ETF.
    /// </summary>
    Task<EtfDto> CreateAsync(ExchangeTradedFund etf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ETF.
    /// </summary>
    Task<EtfDto?> UpdateAsync(Guid id, ExchangeTradedFund etf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an ETF by ID.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a constituent to an ETF.
    /// </summary>
    Task<EtfConstituentDto> AddConstituentAsync(Guid etfId, Guid securityId, decimal weight, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a constituent from an ETF.
    /// </summary>
    Task<bool> RemoveConstituentAsync(Guid etfId, Guid constituentId, CancellationToken cancellationToken = default);
}
