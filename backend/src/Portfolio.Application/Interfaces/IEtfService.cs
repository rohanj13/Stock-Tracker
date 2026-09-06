using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

/// <summary>
/// Defines CRUD operations for managing exchange traded funds and their constituents.
/// </summary>
public interface IEtfService
{
    Task<IReadOnlyCollection<EtfDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EtfDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EtfDto> CreateAsync(CreateEtfRequest request, CancellationToken cancellationToken = default);

    Task<EtfDto?> UpdateAsync(Guid id, UpdateEtfRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EtfConstituentDto> AddConstituentAsync(Guid etfId, AddEtfConstituentRequest request, CancellationToken cancellationToken = default);

    Task<bool> RemoveConstituentAsync(Guid etfId, Guid constituentId, CancellationToken cancellationToken = default);
}
