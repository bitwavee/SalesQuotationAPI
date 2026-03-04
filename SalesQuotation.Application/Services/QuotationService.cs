using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IQuotationService for handling quotation operations
/// </summary>
public class QuotationService : IQuotationService
{
    private readonly ILogger<QuotationService> _logger;

    public QuotationService(ILogger<QuotationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<IEnumerable<QuotationDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all quotations");
        throw new NotImplementedException();
    }

    public Task<QuotationDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting quotation with ID: {QuotationId}", id);
        throw new NotImplementedException();
    }

    public Task<QuotationDto> CreateAsync(CreateQuotationDto dto)
    {
        _logger.LogInformation("Creating new quotation");
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, UpdateQuotationDto dto)
    {
        _logger.LogInformation("Updating quotation with ID: {QuotationId}", id);
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting quotation with ID: {QuotationId}", id);
        throw new NotImplementedException();
    }
}
