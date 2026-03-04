using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling material management operations
/// </summary>
public interface IMaterialService
{
    /// <summary>
    /// Get all materials
    /// </summary>
    Task<IEnumerable<MaterialDto>> GetAllAsync();

    /// <summary>
    /// Get material by ID
    /// </summary>
    Task<MaterialDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Create a new material
    /// </summary>
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto);

    /// <summary>
    /// Update material
    /// </summary>
    Task UpdateAsync(Guid id, UpdateMaterialDto dto);

    /// <summary>
    /// Delete material
    /// </summary>
    Task DeleteAsync(Guid id);
}
