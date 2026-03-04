using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

/// <summary>
/// Controller for quotation management including PDF generation
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotationController : ControllerBase
{
    private readonly IQuotationService _quotationService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<QuotationController> _logger;

    public QuotationController(IQuotationService quotationService, IPdfService pdfService, ILogger<QuotationController> logger)
    {
        _quotationService = quotationService;
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Get all quotations for enquiry
    /// </summary>
    [HttpGet("enquiry/{enquiryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<QuotationDto>>>> GetEnquiryQuotations(Guid enquiryId)
    {
        try
        {
            var quotations = await _quotationService.GetQuotationsByEnquiryAsync(enquiryId);
            return Ok(new ApiResponse<IEnumerable<QuotationDto>>
            {
                Success = true,
                Data = quotations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching quotations");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching quotations",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get quotation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<QuotationDto>>> GetQuotationById(Guid id)
    {
        try
        {
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            
            if (quotation == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Quotation not found",
                    Code = "NOT_FOUND"
                });
            }

            return Ok(new ApiResponse<QuotationDto>
            {
                Success = true,
                Data = quotation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching quotation");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while fetching quotation",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Create new quotation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuotationDto>>> CreateQuotation([FromBody] CreateQuotationDto request)
    {
        try
        {
            var quotation = await _quotationService.CreateQuotationAsync(request);
            return CreatedAtAction(nameof(GetQuotationById), new { id = quotation.Id }, new ApiResponse<QuotationDto>
            {
                Success = true,
                Data = quotation
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
            _logger.LogError(ex, "Error creating quotation");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while creating quotation",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Update quotation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateQuotation(Guid id, [FromBody] UpdateQuotationDto request)
    {
        try
        {
            await _quotationService.UpdateQuotationAsync(id, request);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Quotation updated successfully" }
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
            _logger.LogError(ex, "Error updating quotation");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while updating quotation",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Delete quotation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteQuotation(Guid id)
    {
        try
        {
            await _quotationService.DeleteQuotationAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Quotation deleted successfully" }
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
            _logger.LogError(ex, "Error deleting quotation");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while deleting quotation",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Generate PDF for quotation
    /// </summary>
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GenerateQuotationPdf(Guid id)
    {
        try
        {
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (quotation == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Quotation_{quotation.QuotationNumber}.pdf");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Download quotation PDF
    /// </summary>
    [HttpGet("{id}/download-pdf")]
    public async Task<IActionResult> DownloadQuotationPdf(Guid id)
    {
        try
        {
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (quotation == null || string.IsNullOrWhiteSpace(quotation.PdfPath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", quotation.PdfPath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Quotation_{quotation.QuotationNumber}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading PDF");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Send quotation (mark as sent)
    /// </summary>
    [HttpPost("{id}/send")]
    public async Task<ActionResult<ApiResponse<object>>> SendQuotation(Guid id)
    {
        try
        {
            // Generate PDF if not exists
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (quotation == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Quotation not found",
                    Code = "NOT_FOUND"
                });
            }

            if (string.IsNullOrWhiteSpace(quotation.PdfPath))
            {
                var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(id);
                var pdfPath = await _pdfService.SavePdfAsync(pdfBytes, $"Quotation_{quotation.QuotationNumber}.pdf");
                
                var updateDto = new UpdateQuotationDto 
                { 
                    Status = "SENT"
                };
                await _quotationService.UpdateQuotationAsync(id, updateDto);
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { Message = "Quotation sent successfully" }
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
            _logger.LogError(ex, "Error sending quotation");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while sending quotation",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
