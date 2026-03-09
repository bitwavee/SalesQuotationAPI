using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IQuotationService for handling quotation operations
/// </summary>
public class QuotationService : IQuotationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<QuotationService> _logger;

    public QuotationService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser, ILogger<QuotationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<QuotationDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all quotations");
        
        var quotations = await _context.Quotations
            .Include(q => q.Enquiry)
            .Include(q => q.Items)
            .ToListAsync();

        return _mapper.Map<IEnumerable<QuotationDto>>(quotations);
    }

    public async Task<IEnumerable<QuotationDto>> GetQuotationsByEnquiryAsync(Guid enquiryId)
    {
        _logger.LogInformation("Getting quotations for enquiry: {EnquiryId}", enquiryId);
        
        var quotations = await _context.Quotations
            .Include(q => q.Enquiry)
            .Include(q => q.Items)
            .Where(q => q.EnquiryId == enquiryId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<QuotationDto>>(quotations);
    }

    public async Task<QuotationDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting quotation with ID: {QuotationId}", id);
        
        var quotation = await _context.Quotations
            .Include(q => q.Enquiry)
            .Include(q => q.Items)
            .FirstOrDefaultAsync(q => q.Id == id);

        return quotation != null ? _mapper.Map<QuotationDto>(quotation) : null;
    }

    public async Task<QuotationDto?> GetQuotationByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<QuotationDto> CreateAsync(CreateQuotationDto dto)
    {
        return await CreateQuotationAsync(dto);
    }

    public async Task<QuotationDto> CreateQuotationAsync(CreateQuotationDto dto)
    {
        _logger.LogInformation("Creating new quotation for enquiry: {EnquiryId}", dto.EnquiryId);
        
        var enquiry = await _context.Enquiries.FindAsync(dto.EnquiryId);
        if (enquiry == null || enquiry.IsDeleted)
        {
            throw new KeyNotFoundException($"Enquiry not found: {dto.EnquiryId}");
        }

        var quotation = new Quotation
        {
            Id = Guid.NewGuid(),
            EnquiryId = dto.EnquiryId,
            QuotationNumber = dto.QuotationNumber,
            QuotationDate = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddDays(30), // Valid for 30 days
            Subtotal = 0,
            TaxPercentage = dto.TaxPercentage,
            TaxAmount = 0,
            TotalAmount = 0,
            Notes = dto.Notes,
            Status = "DRAFT",
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Quotation created successfully: {QuotationId}", quotation.Id);

        return _mapper.Map<QuotationDto>(quotation);
    }

    public async Task UpdateAsync(Guid id, UpdateQuotationDto dto)
    {
        await UpdateQuotationAsync(id, dto);
    }

    public async Task UpdateQuotationAsync(Guid id, UpdateQuotationDto dto)
    {
        _logger.LogInformation("Updating quotation with ID: {QuotationId}", id);
        
        var quotation = await _context.Quotations
            .Include(q => q.Items)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quotation == null)
        {
            throw new KeyNotFoundException($"Quotation not found: {id}");
        }

        if (!string.IsNullOrWhiteSpace(dto.QuotationNumber))
            quotation.QuotationNumber = dto.QuotationNumber;
        if (dto.TaxPercentage.HasValue)
            quotation.TaxPercentage = dto.TaxPercentage.Value;
        if (!string.IsNullOrWhiteSpace(dto.Notes))
            quotation.Notes = dto.Notes;
        if (!string.IsNullOrWhiteSpace(dto.Status))
            quotation.Status = dto.Status;

        // Recalculate totals if items exist
        if (quotation.Items.Any())
        {
            quotation.Subtotal = quotation.Items.Sum(i => i.LineTotal);
            quotation.TaxAmount = quotation.Subtotal * (quotation.TaxPercentage / 100);
            quotation.TotalAmount = quotation.Subtotal + (quotation.TaxAmount ?? 0);
        }

        quotation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Quotation updated successfully: {QuotationId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteQuotationAsync(id);
    }

    public async Task DeleteQuotationAsync(Guid id)
    {
        _logger.LogInformation("Deleting quotation with ID: {QuotationId}", id);
        
        var quotation = await _context.Quotations.FindAsync(id);

        if (quotation == null)
        {
            throw new KeyNotFoundException($"Quotation not found: {id}");
        }

        _context.Quotations.Remove(quotation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Quotation deleted successfully: {QuotationId}", id);
    }
}
