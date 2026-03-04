using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for file upload management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Upload file for enquiry
    /// </summary>
    [HttpPost("upload/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<FileUploadDto>>> UploadFile(Guid enquiryId, IFormFile file, [FromQuery] string category = "ATTACHMENT")
    {
        try
        {
            var fileUpload = await _fileService.UploadAsync(file, enquiryId, category);
            return Ok(new ApiResponse<FileUploadDto>
            {
                Success = true,
                Data = fileUpload
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
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Code = "INVALID_FILE"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while uploading file",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get file by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FileUploadDto>>> GetFileById(Guid id)
    {
        try
        {
            var file = await _fileService.GetByIdAsync(id);
            
            if (file == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "File not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<FileUploadDto>
            {
                Success = true,
                Data = file
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching file");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching file",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get all files for enquiry
    /// </summary>
    [HttpGet("enquiry/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FileUploadDto>>>> GetEnquiryFiles(Guid enquiryId)
    {
        try
        {
            var files = await _fileService.GetEnquiryFilesAsync(enquiryId);
            return Ok(new ApiResponse<IEnumerable<FileUploadDto>>
            {
                Success = true,
                Data = files
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enquiry files");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching enquiry files",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Download file
    /// </summary>
    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        try
        {
            var fileInfo = await _fileService.GetByIdAsync(id);
            if (fileInfo == null)
            {
                return NotFound();
            }

            var fileContent = await _fileService.GetFileContentAsync(id);
            return File(fileContent, fileInfo.FileType, fileInfo.FileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Delete file
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFile(Guid id)
    {
        try
        {
            await _fileService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "File deleted successfully" }
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
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting file",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
