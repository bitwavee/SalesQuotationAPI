using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Domain.Enums;
using SalesQuotation.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IStaffService for staff management
/// </summary>
public class StaffService : IStaffService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<StaffService> _logger;

    public StaffService(ApplicationDbContext context, IMapper mapper, ILogger<StaffService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllStaffAsync()
    {
        var staff = await _context.Users
            .Where(u => u.Role == UserRole.Staff && !u.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(staff);
    }

    public async Task<UserDto?> GetStaffByIdAsync(Guid id)
    {
        var staff = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Staff && !u.IsDeleted);

        return staff != null ? _mapper.Map<UserDto>(staff) : null;
    }

    public async Task<UserDto> CreateStaffAsync(CreateUserDto dto)
    {
        _logger.LogInformation($"Creating new staff member: {dto.Email}");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var staff = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = HashPassword(dto.Password),
            Role = UserRole.Staff,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(staff);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Staff member created successfully: {staff.Id}");

        return _mapper.Map<UserDto>(staff);
    }

    public async Task UpdateStaffAsync(Guid id, UpdateUserDto dto)
    {
        _logger.LogInformation($"Updating staff member: {id}");

        var staff = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Staff && !u.IsDeleted);

        if (staff == null)
        {
            throw new KeyNotFoundException($"Staff member not found: {id}");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
            staff.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Phone))
            staff.Phone = dto.Phone;
        if (dto.IsActive.HasValue)
            staff.IsActive = dto.IsActive.Value;

        staff.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Staff member updated successfully: {id}");
    }

    public async Task DeleteStaffAsync(Guid id)
    {
        _logger.LogInformation($"Deleting staff member: {id}");

        var staff = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Staff && !u.IsDeleted);

        if (staff == null)
        {
            throw new KeyNotFoundException($"Staff member not found: {id}");
        }

        staff.IsDeleted = true;
        staff.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Staff member deleted successfully: {id}");
    }

    public async Task AssignEnquiryToStaffAsync(Guid enquiryId, Guid staffId)
    {
        _logger.LogInformation($"Assigning enquiry {enquiryId} to staff {staffId}");

        var enquiry = await _context.Enquiries.FindAsync(enquiryId);
        if (enquiry == null)
        {
            throw new KeyNotFoundException($"Enquiry not found: {enquiryId}");
        }

        var staff = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == staffId && u.Role == UserRole.Staff && !u.IsDeleted);

        if (staff == null)
        {
            throw new KeyNotFoundException($"Staff member not found: {staffId}");
        }

        enquiry.AssignedStaffId = staffId;
        enquiry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Enquiry assigned successfully");
    }

    private string HashPassword(string password)
    {
        using (var hmac = new HMACSHA256())
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hmac.Key) + ":" + Convert.ToBase64String(hash);
        }
    }
}
