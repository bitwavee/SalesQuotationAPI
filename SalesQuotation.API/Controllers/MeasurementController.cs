using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for measurement operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementService _measurementService;
    private readonly IMeasurementConversionService _conversionService;
    private readonly ILogger<MeasurementController> _logger;

    public MeasurementController(IMeasurementService measurementService, IMeasurementConversionService conversionService, ILogger<MeasurementController> logger)
    {
        _measurementService = measurementService;
        _conversionService = conversionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all measurements for enquiry
    /// </summary>
    [HttpGet("enquiry/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MeasurementDto>>>> GetEnquiryMeasurements(Guid enquiryId)
    {
        try
        {
            var measurements = await _measurementService.GetMeasurementsByEnquiryAsync(enquiryId);
            return Ok(new ApiResponse<IEnumerable<MeasurementDto>>
            {
                Success = true,
                Data = measurements
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching measurements");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching measurements",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get measurement by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MeasurementDto>>> GetMeasurementById(Guid id)
    {
        try
        {
            var measurement = await _measurementService.GetMeasurementByIdAsync(id);
            
            if (measurement == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Measurement not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<MeasurementDto>
            {
                Success = true,
                Data = measurement
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching measurement");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching measurement",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Create new measurement
    /// </summary>
    [HttpPost("{enquiryId}")]
    public async Task<ActionResult<ApiResponse<MeasurementDto>>> CreateMeasurement(Guid enquiryId, [FromBody] CreateMeasurementDto request)
    {
        try
        {
            var measurement = await _measurementService.CreateMeasurementAsync(enquiryId, request);
            return CreatedAtAction(nameof(GetMeasurementById), new { id = measurement.Id }, new ApiResponse<MeasurementDto>
            {
                Success = true,
                Data = measurement
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Code = "NOT_FOUND"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating measurement");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while creating measurement",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update measurement
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMeasurement(Guid id, [FromBody] UpdateMeasurementDto request)
    {
        try
        {
            await _measurementService.UpdateMeasurementAsync(id, request);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Measurement updated successfully" }
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Code = "NOT_FOUND"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating measurement");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating measurement",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Delete measurement
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMeasurement(Guid id)
    {
        try
        {
            await _measurementService.DeleteMeasurementAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Measurement deleted successfully" }
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Code = "NOT_FOUND"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting measurement");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting measurement",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Convert measurement units
    /// </summary>
    [HttpPost("convert")]
    public ActionResult<ApiResponse<dynamic>> ConvertMeasurement([FromBody] dynamic conversionRequest)
    {
        try
        {
            // Example: { "type": "meterToSqFt", "length": 10, "breadth": 5 }
            return Ok(new ApiResponse<dynamic>
            {
                Success = true,
                Data = new { Message = "Use specific endpoints for conversion" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting measurement");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while converting measurement",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Convert square meters to square feet
    /// </summary>
    [HttpPost("convert/meter-to-sqft")]
    public ActionResult<ApiResponse<dynamic>> ConvertMeterToSquareFeet([FromBody] dynamic request)
    {
        try
        {
            decimal length = request.length;
            decimal breadth = request.breadth;

            var result = _conversionService.ConvertMeterToSquareFeet(length, breadth);

            return Ok(new ApiResponse<dynamic>
            {
                Success = true,
                Data = new 
                { 
                    Length = length,
                    Breadth = breadth,
                    SquareMeters = length * breadth,
                    SquareFeet = result
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting meters to square feet");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred during conversion",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
