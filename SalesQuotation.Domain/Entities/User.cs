using SalesQuotation.Domain.Enums;

namespace SalesQuotation.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Staff;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<Enquiry> CreatedEnquiries { get; set; } = new List<Enquiry>();
    public ICollection<Enquiry> AssignedEnquiries { get; set; } = new List<Enquiry>();
    public ICollection<Quotation> CreatedQuotations { get; set; } = new List<Quotation>();
    public ICollection<Material> CreatedMaterials { get; set; } = new List<Material>();
}