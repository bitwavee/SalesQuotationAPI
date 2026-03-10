namespace SalesQuotation.Domain.Entities;

public class Material
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal BaseCost { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User? CreatedBy { get; set; }
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}