namespace SalesQuotation.Domain.Entities;

public class EnquiryStatusConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StatusName { get; set; } = string.Empty;
    public string StatusValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? ColorHex { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? CreatedBy { get; set; }
    public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
}