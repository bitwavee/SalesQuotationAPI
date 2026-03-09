using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IMaterialService for material management
/// </summary>
public class MaterialService : IMaterialService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<MaterialService> _logger;

    public MaterialService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser, ILogger<MaterialService> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IEnumerable<MaterialDto>> GetAllAsync()
    {
        var materials = await _context.Materials
            .Where(m => m.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<MaterialDto>>(materials);
    }

    public async Task<MaterialDto?> GetByIdAsync(Guid id)
    {
        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        return material != null ? _mapper.Map<MaterialDto>(material) : null;
    }

    public async Task<MaterialDto> CreateAsync(CreateMaterialDto dto)
    {
        _logger.LogInformation($"Creating new material: {dto.Name}");

        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Unit = dto.Unit,
            BaseCost = dto.BaseCost,
            IsActive = true,
            CreatedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Material created successfully: {material.Id}");

        return _mapper.Map<MaterialDto>(material);
    }

    public async Task UpdateAsync(Guid id, UpdateMaterialDto dto)
    {
        _logger.LogInformation($"Updating material: {id}");

        var material = await _context.Materials.FindAsync(id);

        if (material == null)
        {
            throw new KeyNotFoundException($"Material not found: {id}");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
            material.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Description))
            material.Description = dto.Description;
        if (!string.IsNullOrWhiteSpace(dto.Unit))
            material.Unit = dto.Unit;
        if (dto.BaseCost.HasValue)
            material.BaseCost = dto.BaseCost.Value;
        if (dto.IsActive.HasValue)
            material.IsActive = dto.IsActive.Value;

        material.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Material updated successfully: {id}");
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"Deleting material: {id}");

        var material = await _context.Materials.FindAsync(id);

        if (material == null)
        {
            throw new KeyNotFoundException($"Material not found: {id}");
        }

        material.IsActive = false;
        material.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Material deleted successfully: {id}");
    }
}
