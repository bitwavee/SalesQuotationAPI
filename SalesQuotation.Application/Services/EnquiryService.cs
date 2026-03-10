using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IEnquiryService for handling enquiry operations
/// </summary>
public class EnquiryService : IEnquiryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<EnquiryService> _logger;

    public EnquiryService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser, ILogger<EnquiryService> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<EnquiryDto>> GetAllAsync(string? status = null)
    {
        _logger.LogInformation("Getting all enquiries");

        var query = _context.Enquiries
            .Include(e => e.AssignedStaff)
            .Include(e => e.Measurements)
            .Include(e => e.Quotations)
            .Where(e => !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        var enquiries = await query.ToListAsync();

        return _mapper.Map<IEnumerable<EnquiryDto>>(enquiries);
    }

    public async Task<IEnumerable<EnquiryDto>> GetStaffEnquiriesAsync(Guid staffId, string? status = null)
    {
        _logger.LogInformation("Getting enquiries for staff: {StaffId}", staffId);

        var query = _context.Enquiries
            .Include(e => e.AssignedStaff)
            .Include(e => e.Measurements)
            .Include(e => e.Quotations)
            .Where(e => e.AssignedStaffId == staffId && !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        var enquiries = await query.ToListAsync();

        return _mapper.Map<IEnumerable<EnquiryDto>>(enquiries);
    }

    public async Task<EnquiryDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting enquiry with ID: {EnquiryId}", id);
        
        var enquiry = await _context.Enquiries
            .Include(e => e.AssignedStaff)
            .Include(e => e.Measurements)
            .Include(e => e.Quotations)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        return enquiry != null ? _mapper.Map<EnquiryDto>(enquiry) : null;
    }

    public async Task<EnquiryDto> CreateAsync(CreateEnquiryDto dto)
    {
        _logger.LogInformation("Creating new enquiry for customer: {CustomerName}", dto.CustomerName);
        
        var enquiry = new Enquiry
        {
            Id = Guid.NewGuid(),
            EnquiryNumber = GenerateEnquiryNumber(),
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            CustomerAddress = dto.CustomerAddress,
            Status = "INITIATED",
            Notes = dto.Notes,
            PackageTitle = dto.PackageTitle,
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Enquiries.Add(enquiry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Enquiry created successfully: {EnquiryId}", enquiry.Id);

        return _mapper.Map<EnquiryDto>(enquiry);
    }

    public async Task UpdateAsync(Guid id, UpdateEnquiryDto dto)
    {
        _logger.LogInformation("Updating enquiry with ID: {EnquiryId}", id);
        
        var enquiry = await _context.Enquiries.FindAsync(id);

        if (enquiry == null || enquiry.IsDeleted)
        {
            throw new KeyNotFoundException($"Enquiry not found: {id}");
        }

        if (!string.IsNullOrWhiteSpace(dto.CustomerName))
            enquiry.CustomerName = dto.CustomerName;
        if (!string.IsNullOrWhiteSpace(dto.CustomerEmail))
            enquiry.CustomerEmail = dto.CustomerEmail;
        if (!string.IsNullOrWhiteSpace(dto.CustomerPhone))
            enquiry.CustomerPhone = dto.CustomerPhone;
        if (!string.IsNullOrWhiteSpace(dto.CustomerAddress))
            enquiry.CustomerAddress = dto.CustomerAddress;
        if (!string.IsNullOrWhiteSpace(dto.Status))
            enquiry.Status = dto.Status;
        if (!string.IsNullOrWhiteSpace(dto.Notes))
            enquiry.Notes = dto.Notes;

        enquiry.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Enquiry updated successfully: {EnquiryId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting enquiry with ID: {EnquiryId}", id);
        
        var enquiry = await _context.Enquiries.FindAsync(id);

        if (enquiry == null || enquiry.IsDeleted)
        {
            throw new KeyNotFoundException($"Enquiry not found: {id}");
        }

        enquiry.IsDeleted = true;
        enquiry.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Enquiry deleted successfully: {EnquiryId}", id);
    }

    public async Task<IEnumerable<EnquiryProgressDto>> GetEnquiryProgressAsync(Guid enquiryId)
    {
        _logger.LogInformation("Getting progress history for enquiry: {EnquiryId}", enquiryId);
        
        var progress = await _context.EnquiryProgress
            .Include(p => p.CreatedBy)
            .Where(p => p.EnquiryId == enquiryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EnquiryProgressDto>>(progress);
    }

    public async Task<EnquiryProgressDto> AddEnquiryProgressAsync(Guid enquiryId, CreateEnquiryProgressDto dto)
    {
        _logger.LogInformation("Adding progress update for enquiry: {EnquiryId}", enquiryId);
        
        var enquiry = await _context.Enquiries.FindAsync(enquiryId);

        if (enquiry == null || enquiry.IsDeleted)
        {
            throw new KeyNotFoundException($"Enquiry not found: {enquiryId}");
        }

        var progress = new EnquiryProgress
        {
            Id = Guid.NewGuid(),
            EnquiryId = enquiryId,
            NewStatus = dto.Status,
            Comment = dto.Notes,
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow
        };

        _context.EnquiryProgress.Add(progress);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Progress added successfully: {ProgressId}", progress.Id);

        return _mapper.Map<EnquiryProgressDto>(progress);
    }

    private string GenerateEnquiryNumber()
    {
        return $"ENQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}
