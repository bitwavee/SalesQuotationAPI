using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling enquiry status configuration (Admin only)
/// </summary>
public interface IEnquiryStatusConfigService
{
    /// <summary>
    /// Get all status configurations
    /// </summary>
    Task<IEnumerable<EnquiryStatusConfigDto>> GetAllAsync();

    /// <summary>
    /// Get status configuration by ID
    /// </summary>
    Task<EnquiryStatusConfigDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Create a new status configuration
    /// </summary>
    Task<EnquiryStatusConfigDto> CreateAsync(CreateEnquiryStatusConfigDto dto);

    /// <summary>
    /// Update status configuration
    /// </summary>
    Task UpdateAsync(Guid id, UpdateEnquiryStatusConfigDto dto);

    /// <summary>
    /// Delete status configuration
    /// </summary>
    Task DeleteAsync(Guid id);
}
