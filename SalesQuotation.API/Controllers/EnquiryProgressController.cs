using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for enquiry progress tracking and comments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnquiryProgressController : ControllerBase
{
    private readonly IEnquiryService _enquiryService;
    private readonly ILogger<EnquiryProgressController> _logger;

    public EnquiryProgressController(IEnquiryService enquiryService, ILogger<EnquiryProgressController> logger)
    {
        _enquiryService = enquiryService;
        _logger = logger;
    }

    /// <summary>
    /// Get progress history for enquiry
    /// </summary>
    [HttpGet("enquiry/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnquiryProgressDto>>>> GetEnquiryProgress(Guid enquiryId)
    {
        try
        {
            var progress = await _enquiryService.GetEnquiryProgressAsync(enquiryId);
            return Ok(new ApiResponse<IEnumerable<EnquiryProgressDto>>
            {
                Success = true,
                Data = progress
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
            _logger.LogError(ex, "Error fetching enquiry progress");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching enquiry progress",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Add progress update to enquiry
    /// </summary>
    [HttpPost("enquiry/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<EnquiryProgressDto>>> AddProgress(Guid enquiryId, [FromBody] CreateEnquiryProgressDto request)
    {
        try
        {
            var progress = await _enquiryService.AddEnquiryProgressAsync(enquiryId, request);
            return Ok(new ApiResponse<EnquiryProgressDto>
            {
                Success = true,
                Data = progress
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
            _logger.LogError(ex, "Error adding progress");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while adding progress",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update enquiry status
    /// </summary>
    [HttpPost("enquiry/{enquiryId}/update-status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEnquiryStatus(Guid enquiryId, [FromBody] CreateEnquiryProgressDto request)
    {
        try
        {
            // Update enquiry status
            var updateDto = new UpdateEnquiryDto { Status = request.Status };
            await _enquiryService.UpdateAsync(enquiryId, updateDto);

            // Add progress record
            await _enquiryService.AddEnquiryProgressAsync(enquiryId, request);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Status updated successfully" }
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
            _logger.LogError(ex, "Error updating status");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating status",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
