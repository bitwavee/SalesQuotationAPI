using System.Diagnostics.Metrics;

namespace SalesQuotation.Domain.Entities;

public class Enquiry
{
    public Guid Id { get; set; }
    public string EnquiryNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string Status { get; set; } = "INITIATED";
    public string? Notes { get; set; }
    public string? PackageTitle { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? CreatedBy { get; set; }
    public User? AssignedStaff { get; set; }
    public EnquiryStatusConfig? StatusConfig { get; set; }
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    public ICollection<EnquiryProgress> ProgressHistory { get; set; } = new List<EnquiryProgress>();
    public ICollection<FileUpload> Attachments { get; set; } = new List<FileUpload>();
}