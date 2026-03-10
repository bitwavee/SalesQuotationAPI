using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IEnquiryStatusConfigService
/// </summary>
public class EnquiryStatusConfigService : IEnquiryStatusConfigService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<EnquiryStatusConfigService> _logger;

    public EnquiryStatusConfigService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser, ILogger<EnquiryStatusConfigService> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IEnumerable<EnquiryStatusConfigDto>> GetAllAsync()
    {
        var configs = await _context.EnquiryStatusConfigs
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EnquiryStatusConfigDto>>(configs);
    }

    public async Task<EnquiryStatusConfigDto?> GetByIdAsync(Guid id)
    {
        var config = await _context.EnquiryStatusConfigs.FindAsync(id);
        return config != null ? _mapper.Map<EnquiryStatusConfigDto>(config) : null;
    }

    public async Task<EnquiryStatusConfigDto> CreateAsync(CreateEnquiryStatusConfigDto dto)
    {
        _logger.LogInformation($"Creating new status config: {dto.StatusName}");

        if (await _context.EnquiryStatusConfigs.AnyAsync(c => c.StatusValue == dto.StatusKey))
        {
            throw new InvalidOperationException($"Status value already exists: {dto.StatusKey}");
        }

        var config = new EnquiryStatusConfig
        {
            Id = Guid.NewGuid(),
            StatusName = dto.StatusName,
            StatusValue = dto.StatusKey,
            DisplayOrder = dto.DisplayOrder,
            ColorHex = dto.Color,
            IsActive = true,
            RequiredFields = dto.RequiredFields,
            FieldPermissions = dto.FieldPermissions,
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.EnquiryStatusConfigs.Add(config);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Status config created successfully: {config.Id}");

        return _mapper.Map<EnquiryStatusConfigDto>(config);
    }

    public async Task UpdateAsync(Guid id, UpdateEnquiryStatusConfigDto dto)
    {
        _logger.LogInformation($"Updating status config: {id}");

        var config = await _context.EnquiryStatusConfigs.FindAsync(id);

        if (config == null)
        {
            throw new KeyNotFoundException($"Status config not found: {id}");
        }

        if (!string.IsNullOrWhiteSpace(dto.StatusName))
            config.StatusName = dto.StatusName;
        if (!string.IsNullOrWhiteSpace(dto.StatusKey))
            config.StatusValue = dto.StatusKey;
        if (dto.DisplayOrder.HasValue)
            config.DisplayOrder = dto.DisplayOrder.Value;
        if (!string.IsNullOrWhiteSpace(dto.Color))
            config.ColorHex = dto.Color;
        if (dto.IsActive.HasValue)
            config.IsActive = dto.IsActive.Value;
        if (dto.RequiredFields != null)
            config.RequiredFields = dto.RequiredFields;
        if (dto.FieldPermissions != null)
            config.FieldPermissions = dto.FieldPermissions;

        config.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Status config updated successfully: {id}");
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"Deleting status config: {id}");

        var config = await _context.EnquiryStatusConfigs.FindAsync(id);

        if (config == null)
        {
            throw new KeyNotFoundException($"Status config not found: {id}");
        }

        config.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Status config deleted successfully: {id}");
    }
}
