using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for material management (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<MaterialController> _logger;

    public MaterialController(IMaterialService materialService, ILogger<MaterialController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    /// <summary>
    /// Get all materials
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialDto>>>> GetAllMaterials()
    {
        try
        {
            var materials = await _materialService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<MaterialDto>>
            {
                Success = true,
                Data = materials
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching materials");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching materials",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get material by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> GetMaterialById(Guid id)
    {
        try
        {
            var material = await _materialService.GetByIdAsync(id);
            
            if (material == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Material not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<MaterialDto>
            {
                Success = true,
                Data = material
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching material");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching material",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Create new material (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> CreateMaterial([FromBody] CreateMaterialDto request)
    {
        try
        {
            var material = await _materialService.CreateAsync(request);
            return CreatedAtAction(nameof(GetMaterialById), new { id = material.Id }, new ApiResponse<MaterialDto>
            {
                Success = true,
                Data = material
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating material");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while creating material",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update material (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMaterial(Guid id, [FromBody] UpdateMaterialDto request)
    {
        try
        {
            await _materialService.UpdateAsync(id, request);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Material updated successfully" }
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
            _logger.LogError(ex, "Error updating material");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating material",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Delete material (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMaterial(Guid id)
    {
        try
        {
            await _materialService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Material deleted successfully" }
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
            _logger.LogError(ex, "Error deleting material");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting material",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
