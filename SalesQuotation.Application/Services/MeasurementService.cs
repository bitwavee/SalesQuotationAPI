using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;
using System.Text.Json;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IMeasurementService for handling measurement operations
/// </summary>
public class MeasurementService : IMeasurementService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<MeasurementService> _logger;

    public MeasurementService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser, ILogger<MeasurementService> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<MeasurementDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all measurements");

        var measurements = await _context.Measurements
            .Include(m => m.Category)
            .ToListAsync();

        return _mapper.Map<IEnumerable<MeasurementDto>>(measurements);
    }

    public async Task<IEnumerable<MeasurementDto>> GetMeasurementsByEnquiryAsync(Guid enquiryId)
    {
        _logger.LogInformation("Getting measurements for enquiry: {EnquiryId}", enquiryId);

        var measurements = await _context.Measurements
            .Include(m => m.Category)
            .Where(m => m.EnquiryId == enquiryId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<MeasurementDto>>(measurements);
    }

    public async Task<MeasurementDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting measurement with ID: {MeasurementId}", id);

        var measurement = await _context.Measurements
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        return measurement != null ? _mapper.Map<MeasurementDto>(measurement) : null;
    }

    public async Task<MeasurementDto?> GetMeasurementByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto)
    {
        _logger.LogInformation("Creating new measurement");
        throw new NotImplementedException("Use CreateMeasurementAsync with enquiryId instead");
    }

    public async Task<MeasurementDto> CreateMeasurementAsync(Guid enquiryId, CreateMeasurementDto dto)
    {
        _logger.LogInformation("Creating new measurement for enquiry: {EnquiryId}", enquiryId);

        var enquiry = await _context.Enquiries.FindAsync(enquiryId);
        if (enquiry == null || enquiry.IsDeleted)
        {
            throw new KeyNotFoundException($"Enquiry not found: {enquiryId}");
        }

        var category = await _context.MeasurementCategories.FindAsync(dto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Measurement category not found: {dto.CategoryId}");
        }

        // Serialize measurement data to JSON
        var measurementDataJson = JsonSerializer.Serialize(dto.MeasurementData);

        // Calculate value (example: length * breadth for area)
        decimal calculatedValue = 0;
        if (dto.MeasurementData.Count >= 2)
        {
            var values = dto.MeasurementData.Values.ToList();
            calculatedValue = values[0] * values[1];
        }

        var measurement = new Measurement
        {
            Id = Guid.NewGuid(),
            EnquiryId = enquiryId,
            CategoryId = dto.CategoryId,
            MeasurementData = measurementDataJson,
            CalculatedValue = calculatedValue,
            Notes = dto.Notes,
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Measurement created successfully: {MeasurementId}", measurement.Id);

        return _mapper.Map<MeasurementDto>(measurement);
    }

    public async Task UpdateAsync(Guid id, UpdateMeasurementDto dto)
    {
        await UpdateMeasurementAsync(id, dto);
    }

    public async Task UpdateMeasurementAsync(Guid id, UpdateMeasurementDto dto)
    {
        _logger.LogInformation("Updating measurement with ID: {MeasurementId}", id);

        var measurement = await _context.Measurements.FindAsync(id);

        if (measurement == null)
        {
            throw new KeyNotFoundException($"Measurement not found: {id}");
        }

        if (dto.CategoryId.HasValue)
        {
            var category = await _context.MeasurementCategories.FindAsync(dto.CategoryId.Value);
            if (category == null)
            {
                throw new KeyNotFoundException($"Measurement category not found: {dto.CategoryId}");
            }
            measurement.CategoryId = dto.CategoryId.Value;
        }

        if (dto.MeasurementData != null && dto.MeasurementData.Count > 0)
        {
            measurement.MeasurementData = JsonSerializer.Serialize(dto.MeasurementData);

            // Recalculate value
            var values = dto.MeasurementData.Values.ToList();
            if (values.Count >= 2)
            {
                measurement.CalculatedValue = values[0] * values[1];
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.Notes))
            measurement.Notes = dto.Notes;

        measurement.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Measurement updated successfully: {MeasurementId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteMeasurementAsync(id);
    }

    public async Task DeleteMeasurementAsync(Guid id)
    {
        _logger.LogInformation("Deleting measurement with ID: {MeasurementId}", id);

        var measurement = await _context.Measurements.FindAsync(id);

        if (measurement == null)
        {
            throw new KeyNotFoundException($"Measurement not found: {id}");
        }

        _context.Measurements.Remove(measurement);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Measurement deleted successfully: {MeasurementId}", id);
    }
}

