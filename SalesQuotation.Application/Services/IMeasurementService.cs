using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling measurement operations
/// </summary>
public interface IMeasurementService
{
    Task<IEnumerable<MeasurementDto>> GetAllAsync();
    Task<MeasurementDto?> GetByIdAsync(Guid id);
    Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto);
    Task UpdateAsync(Guid id, UpdateMeasurementDto dto);
    Task DeleteAsync(Guid id);
}
