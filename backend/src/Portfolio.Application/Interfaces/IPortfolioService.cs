using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Defines CRUD operations for managing portfolios and their positions.
/// </summary>
public interface IPortfolioService
{
    Task<IReadOnlyCollection<PortfolioDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PortfolioDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PortfolioDto> CreateAsync(CreatePortfolioRequest request, CancellationToken cancellationToken = default);

    Task<PortfolioDto?> UpdateAsync(Guid id, UpdatePortfolioRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PortfolioPositionDto> AddPositionAsync(Guid portfolioId, AddPortfolioPositionRequest request, CancellationToken cancellationToken = default);

    Task<bool> RemovePositionAsync(Guid portfolioId, Guid positionId, CancellationToken cancellationToken = default);
}
