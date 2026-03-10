using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Enums;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(ApplicationDbContext context, IMapper mapper, ILogger<ReportService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync()
    {
        _logger.LogInformation("Generating report summary");

        var totalEnquiries = await _context.Enquiries.CountAsync(e => !e.IsDeleted);
        var totalQuotations = await _context.Quotations.CountAsync();
        var totalStaff = await _context.Users.CountAsync(u => u.Role == UserRole.Staff && !u.IsDeleted);

        var enquiriesByStatus = await _context.Enquiries
            .Where(e => !e.IsDeleted)
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count);

        var recentEnquiries = await _context.Enquiries
            .Include(e => e.AssignedStaff)
            .Include(e => e.Measurements)
            .Include(e => e.Quotations)
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .Take(10)
            .ToListAsync();

        var revenueTotal = await _context.Quotations
            .Where(q => q.Status == "SENT" || q.Status == "ACCEPTED")
            .SumAsync(q => q.TotalAmount);

        return new ReportSummaryDto
        {
            TotalEnquiries = totalEnquiries,
            TotalQuotations = totalQuotations,
            TotalStaff = totalStaff,
            EnquiriesByStatus = enquiriesByStatus,
            RecentEnquiries = _mapper.Map<IEnumerable<EnquiryDto>>(recentEnquiries),
            RevenueTotal = revenueTotal
        };
    }
}
