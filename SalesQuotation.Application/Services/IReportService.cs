using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync();
}
