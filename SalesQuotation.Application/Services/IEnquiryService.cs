using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling enquiry operations
/// </summary>
public interface IEnquiryService
{
    Task<IEnumerable<EnquiryDto>> GetAllAsync();
    Task<EnquiryDto?> GetByIdAsync(Guid id);
    Task<EnquiryDto> CreateAsync(CreateEnquiryDto dto);
    Task UpdateAsync(Guid id, UpdateEnquiryDto dto);
    Task DeleteAsync(Guid id);
}
