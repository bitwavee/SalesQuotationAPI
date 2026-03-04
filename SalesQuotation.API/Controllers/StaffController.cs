using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for staff management (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    /// <summary>
    /// Get all staff members (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllStaff()
    {
        try
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>>
            {
                Success = true,
                Data = staff
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff members");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching staff members",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get staff member by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetStaffById(Guid id)
    {
        try
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            
            if (staff == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Staff member not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = staff
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff member");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching staff member",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Create new staff member (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateStaff([FromBody] CreateUserDto request)
    {
        try
        {
            var staff = await _staffService.CreateStaffAsync(request);
            return CreatedAtAction(nameof(GetStaffById), new { id = staff.Id }, new ApiResponse<UserDto>
            {
                Success = true,
                Data = staff
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
            _logger.LogError(ex, "Error creating staff member");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while creating staff member",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update staff member (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStaff(Guid id, [FromBody] UpdateUserDto request)
    {
        try
        {
            await _staffService.UpdateStaffAsync(id, request);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Staff member updated successfully" }
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
            _logger.LogError(ex, "Error updating staff member");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating staff member",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Delete staff member (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStaff(Guid id)
    {
        try
        {
            await _staffService.DeleteStaffAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Staff member deleted successfully" }
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
            _logger.LogError(ex, "Error deleting staff member");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting staff member",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Assign enquiry to staff (Admin only)
    /// </summary>
    [HttpPost("assign-enquiry")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> AssignEnquiry([FromBody] AssignEnquiryDto request)
    {
        try
        {
            await _staffService.AssignEnquiryToStaffAsync(request.EnquiryId, request.StaffId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Enquiry assigned successfully" }
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
            _logger.LogError(ex, "Error assigning enquiry");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while assigning enquiry",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
