namespace SalesQuotation.Domain.Entities;

public class Measurement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnquiryId { get; set; }
    public Guid CategoryId { get; set; }
    public string MeasurementData { get; set; } = string.Empty;
    public decimal CalculatedValue { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Enquiry? Enquiry { get; set; }
    public MeasurementCategory? Category { get; set; }
    public User? CreatedBy { get; set; }
}