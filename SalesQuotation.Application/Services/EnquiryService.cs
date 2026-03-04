using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IEnquiryService for handling enquiry operations
/// </summary>
public class EnquiryService : IEnquiryService
{
    private readonly ILogger<EnquiryService> _logger;

    public EnquiryService(ILogger<EnquiryService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<IEnumerable<EnquiryDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all enquiries");
        throw new NotImplementedException();
    }

    public Task<EnquiryDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting enquiry with ID: {EnquiryId}", id);
        throw new NotImplementedException();
    }

    public Task<EnquiryDto> CreateAsync(CreateEnquiryDto dto)
    {
        _logger.LogInformation("Creating new enquiry");
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, UpdateEnquiryDto dto)
    {
        _logger.LogInformation("Updating enquiry with ID: {EnquiryId}", id);
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting enquiry with ID: {EnquiryId}", id);
        throw new NotImplementedException();
    }
}
