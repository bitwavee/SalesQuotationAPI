namespace SalesQuotation.Domain.Entities;

public class QuotationItem
{
    public Guid Id { get; set; }
    public Guid QuotationId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Quotation? Quotation { get; set; }
    public Material? Material { get; set; }
}