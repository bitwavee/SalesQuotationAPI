namespace SalesQuotation.Domain.Entities;

public class MeasurementCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryKey { get; set; } = string.Empty;
    public string MeasurementFields { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? CreatedBy { get; set; }
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}