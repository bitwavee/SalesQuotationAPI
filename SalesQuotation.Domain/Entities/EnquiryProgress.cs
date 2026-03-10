namespace SalesQuotation.Domain.Entities;

public class EnquiryProgress
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Enquiry? Enquiry { get; set; }
    public User? CreatedBy { get; set; }
}