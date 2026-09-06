using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Defines CRUD operations for managing stock securities.
/// </summary>
public interface IStockService
{
    Task<IReadOnlyCollection<StockDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<StockDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StockDto> CreateAsync(CreateStockRequest request, CancellationToken cancellationToken = default);

    Task<StockDto?> UpdateAsync(Guid id, UpdateStockRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
