using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling quotation operations
/// </summary>
public interface IQuotationService
{
    Task<IEnumerable<QuotationDto>> GetAllAsync();
    Task<QuotationDto?> GetByIdAsync(Guid id);
    Task<QuotationDto> CreateAsync(CreateQuotationDto dto);
    Task UpdateAsync(Guid id, UpdateQuotationDto dto);
    Task DeleteAsync(Guid id);
}
