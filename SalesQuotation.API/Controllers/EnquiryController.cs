using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for enquiry management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnquiryController : ControllerBase
{
    private readonly IEnquiryService _enquiryService;
    private readonly ILogger<EnquiryController> _logger;

    public EnquiryController(IEnquiryService enquiryService, ILogger<EnquiryController> logger)
    {
        _enquiryService = enquiryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all enquiries (Admin sees all, Staff sees own)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnquiryDto>>>> GetAll([FromQuery] string? status = null)
    {
        try
        {
            var role = User.FindFirst("role")?.Value;
            var userId = User.FindFirst("sub")?.Value;

            IEnumerable<EnquiryDto> enquiries;

            if (role == "Admin")
            {
                enquiries = await _enquiryService.GetAllAsync(status);
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                enquiries = await _enquiryService.GetStaffEnquiriesAsync(Guid.Parse(userId), status);
            }
            else
            {
                return Unauthorized();
            }

            return Ok(new ApiResponse<IEnumerable<EnquiryDto>> { Success = true, Data = enquiries });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enquiries");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching enquiries",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get enquiry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EnquiryDto>>> GetById(Guid id)
    {
        try
        {
            var enquiry = await _enquiryService.GetByIdAsync(id);
            if (enquiry == null)
                return NotFound(new ApiResponse<object> { Success = false, Error = "Enquiry not found", Code = "NOT_FOUND" });

            return Ok(new ApiResponse<EnquiryDto> { Success = true, Data = enquiry });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enquiry");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = "Internal error", Code = "INTERNAL_ERROR" });
        }
    }

    /// <summary>
    /// Create new enquiry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EnquiryDto>>> Create([FromBody] CreateEnquiryDto request)
    {
        try
        {
            var enquiry = await _enquiryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = enquiry.Id }, new ApiResponse<EnquiryDto>
            {
                Success = true,
                Data = enquiry
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enquiry");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = "Internal error", Code = "INTERNAL_ERROR" });
        }
    }

    /// <summary>
    /// Update enquiry
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UpdateEnquiryDto request)
    {
        try
        {
            await _enquiryService.UpdateAsync(id, request);
            return Ok(new ApiResponse<object> { Success = true, Data = new { Message = "Enquiry updated successfully" } });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object> { Success = false, Error = ex.Message, Code = "NOT_FOUND" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enquiry");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = "Internal error", Code = "INTERNAL_ERROR" });
        }
    }

    /// <summary>
    /// Delete enquiry (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _enquiryService.DeleteAsync(id);
            return Ok(new ApiResponse<object> { Success = true, Data = new { Message = "Enquiry deleted successfully" } });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object> { Success = false, Error = ex.Message, Code = "NOT_FOUND" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting enquiry");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = "Internal error", Code = "INTERNAL_ERROR" });
        }
    }
}
