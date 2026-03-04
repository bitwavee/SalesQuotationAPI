using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling measurement operations
/// </summary>
public interface IMeasurementService
{
    Task<IEnumerable<MeasurementDto>> GetAllAsync();
    Task<IEnumerable<MeasurementDto>> GetMeasurementsByEnquiryAsync(Guid enquiryId);
    Task<MeasurementDto?> GetByIdAsync(Guid id);
    Task<MeasurementDto?> GetMeasurementByIdAsync(Guid id);
    Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto);
    Task<MeasurementDto> CreateMeasurementAsync(Guid enquiryId, CreateMeasurementDto dto);
    Task UpdateAsync(Guid id, UpdateMeasurementDto dto);
    Task UpdateMeasurementAsync(Guid id, UpdateMeasurementDto dto);
    Task DeleteAsync(Guid id);
    Task DeleteMeasurementAsync(Guid id);
}

