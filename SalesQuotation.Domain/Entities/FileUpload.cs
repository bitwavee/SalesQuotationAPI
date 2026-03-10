namespace SalesQuotation.Domain.Entities;

public class FileUpload
{
    public Guid Id { get; set; }
    public Guid? EnquiryId { get; set; }
    public Guid? QuotationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public int? FileSizeBytes { get; set; }
    public string? Category { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Cost { get; set; }
    public Guid UploadedById { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Enquiry? Enquiry { get; set; }
    public Quotation? Quotation { get; set; }
    public User? UploadedBy { get; set; }
}