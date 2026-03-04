using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IMeasurementService for handling measurement operations
/// </summary>
public class MeasurementService : IMeasurementService
{
    private readonly ILogger<MeasurementService> _logger;

    public MeasurementService(ILogger<MeasurementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<IEnumerable<MeasurementDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all measurements");
        throw new NotImplementedException();
    }

    public Task<MeasurementDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting measurement with ID: {MeasurementId}", id);
        throw new NotImplementedException();
    }

    public Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto)
    {
        _logger.LogInformation("Creating new measurement");
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, UpdateMeasurementDto dto)
    {
        _logger.LogInformation("Updating measurement with ID: {MeasurementId}", id);
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting measurement with ID: {MeasurementId}", id);
        throw new NotImplementedException();
    }
}
