namespace SalesQuotation.Application.Dtos;

/// <summary>
/// Standard API response wrapper for all endpoints
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message (only populated if Success is false)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Error code for client-side handling
    /// </summary>
    public string? Code { get; set; }
}

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public UserDto User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "STAFF";
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
}

public class ChangeRoleDto
{
    /// <summary>
    /// Target role: "Admin" or "Staff"
    /// </summary>
    public string Role { get; set; } = string.Empty;
}

public class EnquiryDto
{
    public Guid Id { get; set; }
    public string EnquiryNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public UserDto? AssignedStaff { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int MeasurementsCount { get; set; }
    public int QuotationsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateEnquiryDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string? Notes { get; set; }
}

public class UpdateEnquiryDto
{
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

public class QuotationItemDto
{
    public Guid Id { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
    public string? Notes { get; set; }
}

public class QuotationDto
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PdfPath { get; set; }
    public List<QuotationItemDto> Items { get; set; } = new();
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateQuotationDto
{
    public Guid EnquiryId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public string? Notes { get; set; }
}

public class UpdateQuotationDto
{
    public string? QuotationNumber { get; set; }
    public decimal? TaxPercentage { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}

public class MeasurementCategoryDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryKey { get; set; } = string.Empty;
    public string MeasurementFields { get; set; } = string.Empty;
}

public class MeasurementDto
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public Guid CategoryId { get; set; }
    public MeasurementCategoryDto? Category { get; set; }
    public string MeasurementData { get; set; } = string.Empty;
    public decimal CalculatedValue { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMeasurementDto
{
    public Guid CategoryId { get; set; }
    public Dictionary<string, decimal> MeasurementData { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateMeasurementDto
{
    public Guid? CategoryId { get; set; }
    public Dictionary<string, decimal>? MeasurementData { get; set; }
    public string? Notes { get; set; }
}

public class MaterialDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal BaseCost { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMaterialDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal BaseCost { get; set; }
}

public class UpdateMaterialDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public decimal? BaseCost { get; set; }
    public bool? IsActive { get; set; }
}

public class EnquiryStatusConfigDto
{
    public Guid Id { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusKey { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEnquiryStatusConfigDto
{
    public string StatusName { get; set; } = string.Empty;
    public string StatusKey { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Color { get; set; }
}

public class UpdateEnquiryStatusConfigDto
{
    public string? StatusName { get; set; }
    public string? StatusKey { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Color { get; set; }
    public bool? IsActive { get; set; }
}

public class EnquiryProgressDto
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public UserDto? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEnquiryProgressDto
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class FileUploadDto
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class AssignEnquiryDto
{
    public Guid EnquiryId { get; set; }
    public Guid StaffId { get; set; }
}
