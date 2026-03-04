using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling quotation operations
/// </summary>
public interface IQuotationService
{
    Task<IEnumerable<QuotationDto>> GetAllAsync();
    Task<IEnumerable<QuotationDto>> GetQuotationsByEnquiryAsync(Guid enquiryId);
    Task<QuotationDto?> GetByIdAsync(Guid id);
    Task<QuotationDto?> GetQuotationByIdAsync(Guid id);
    Task<QuotationDto> CreateAsync(CreateQuotationDto dto);
    Task<QuotationDto> CreateQuotationAsync(CreateQuotationDto dto);
    Task UpdateAsync(Guid id, UpdateQuotationDto dto);
    Task UpdateQuotationAsync(Guid id, UpdateQuotationDto dto);
    Task DeleteAsync(Guid id);
    Task DeleteQuotationAsync(Guid id);
}

