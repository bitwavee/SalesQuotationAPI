namespace SalesQuotation.Domain.Entities;

public class MeasurementCategory
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryKey { get; set; } = string.Empty;
    public string MeasurementFields { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User? CreatedBy { get; set; }
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}