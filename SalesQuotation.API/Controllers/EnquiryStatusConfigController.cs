using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for enquiry status configuration (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnquiryStatusConfigController : ControllerBase
{
    private readonly IEnquiryStatusConfigService _statusConfigService;
    private readonly ILogger<EnquiryStatusConfigController> _logger;

    public EnquiryStatusConfigController(IEnquiryStatusConfigService statusConfigService, ILogger<EnquiryStatusConfigController> logger)
    {
        _statusConfigService = statusConfigService;
        _logger = logger;
    }

    /// <summary>
    /// Get all status configurations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnquiryStatusConfigDto>>>> GetAllStatusConfigs()
    {
        try
        {
            var configs = await _statusConfigService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<EnquiryStatusConfigDto>>
            {
                Success = true,
                Data = configs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching status configurations");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching status configurations",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get status configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EnquiryStatusConfigDto>>> GetStatusConfigById(Guid id)
    {
        try
        {
            var config = await _statusConfigService.GetByIdAsync(id);
            
            if (config == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Status configuration not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<EnquiryStatusConfigDto>
            {
                Success = true,
                Data = config
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching status configuration");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching status configuration",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Create new status configuration (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EnquiryStatusConfigDto>>> CreateStatusConfig([FromBody] CreateEnquiryStatusConfigDto request)
    {
        try
        {
            var config = await _statusConfigService.CreateAsync(request);
            return CreatedAtAction(nameof(GetStatusConfigById), new { id = config.Id }, new ApiResponse<EnquiryStatusConfigDto>
            {
                Success = true,
                Data = config
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Code = "BAD_REQUEST"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating status configuration");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while creating status configuration",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update status configuration (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatusConfig(Guid id, [FromBody] UpdateEnquiryStatusConfigDto request)
    {
        try
        {
            await _statusConfigService.UpdateAsync(id, request);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Status configuration updated successfully" }
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
            _logger.LogError(ex, "Error updating status configuration");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating status configuration",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Delete status configuration (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStatusConfig(Guid id)
    {
        try
        {
            await _statusConfigService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Status configuration deleted successfully" }
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
            _logger.LogError(ex, "Error deleting status configuration");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting status configuration",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
