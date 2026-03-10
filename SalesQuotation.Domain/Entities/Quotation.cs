namespace SalesQuotation.Domain.Entities;

public class Quotation
{
    public Guid Id { get; set; }
    public Guid EnquiryId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxPercentage { get; set; } = 0;
    public decimal? TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? PdfPath { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Enquiry? Enquiry { get; set; }
    public User? CreatedBy { get; set; }
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    public ICollection<FileUpload> Attachments { get; set; } = new List<FileUpload>();
}