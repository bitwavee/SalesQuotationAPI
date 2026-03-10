using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Get report summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<ReportSummaryDto>>> GetSummary()
    {
        try
        {
            var summary = await _reportService.GetSummaryAsync();
            return Ok(new ApiResponse<ReportSummaryDto>
            {
                Success = true,
                Data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report summary");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while generating report",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Generate report (POST variant)
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<ReportSummaryDto>>> GenerateReport()
    {
        try
        {
            var summary = await _reportService.GetSummaryAsync();
            return Ok(new ApiResponse<ReportSummaryDto>
            {
                Success = true,
                Data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred while generating report",
                Code = "INTERNAL_ERROR"
            });
        }
    }
}
