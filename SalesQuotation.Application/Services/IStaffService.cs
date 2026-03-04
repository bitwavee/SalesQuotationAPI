using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling staff management operations (Admin only)
/// </summary>
public interface IStaffService
{
    /// <summary>
    /// Get all staff members
    /// </summary>
    Task<IEnumerable<UserDto>> GetAllStaffAsync();

    /// <summary>
    /// Get staff member by ID
    /// </summary>
    Task<UserDto?> GetStaffByIdAsync(Guid id);

    /// <summary>
    /// Create a new staff member
    /// </summary>
    Task<UserDto> CreateStaffAsync(CreateUserDto dto);

    /// <summary>
    /// Update staff member
    /// </summary>
    Task UpdateStaffAsync(Guid id, UpdateUserDto dto);

    /// <summary>
    /// Delete staff member
    /// </summary>
    Task DeleteStaffAsync(Guid id);

    /// <summary>
    /// Assign enquiry to staff member
    /// </summary>
    Task AssignEnquiryToStaffAsync(Guid enquiryId, Guid staffId);
}
