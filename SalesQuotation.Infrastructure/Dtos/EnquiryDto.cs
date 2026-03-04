// This file is deprecated. Use SalesQuotation.Application.Dtos instead.
// Keep this file for backwards compatibility but do not add new code here.

namespace SalesQuotation.Infrastructure.Dtos;

// DTOs have been moved to SalesQuotation.Application.Dtos.AllDtos

public class UpdateEnquiryStatusDto
{
    public string NewStatus { get; set; } = string.Empty;
    public string? Comment { get; set; }
}