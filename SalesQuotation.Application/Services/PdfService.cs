using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Infrastructure.Data;
using System.Text;
using System.IO;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IPdfService for PDF generation using simple text format
/// Note: For production, consider using iText7, SelectPdf, or similar modern libraries
/// </summary>
public class PdfService : IPdfService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PdfService> _logger;
    private readonly string _pdfDirectory;

    public PdfService(ApplicationDbContext context, ILogger<PdfService> logger)
    {
        _context = context;
        _logger = logger;
        _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(_pdfDirectory))
        {
            Directory.CreateDirectory(_pdfDirectory);
        }
    }

    public async Task<byte[]> GenerateQuotationPdfAsync(Guid quotationId)
    {
        _logger.LogInformation($"Generating PDF for quotation: {quotationId}");

        var quotation = await _context.Quotations
            .Include(q => q.Enquiry)
            .Include(q => q.Items)
            .ThenInclude(i => i.Material)
            .FirstOrDefaultAsync(q => q.Id == quotationId);

        if (quotation == null)
        {
            throw new KeyNotFoundException($"Quotation not found: {quotationId}");
        }

        // Generate simple text-based quotation
        var sb = new StringBuilder();
        
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("                      QUOTATION                         ");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine();
        
        sb.AppendLine($"Quotation #: {quotation.QuotationNumber}");
        sb.AppendLine($"Date: {quotation.QuotationDate:yyyy-MM-dd}");
        sb.AppendLine($"Valid Until: {quotation.ValidUntil:yyyy-MM-dd}");
        sb.AppendLine($"Enquiry #: {quotation.Enquiry?.EnquiryNumber}");
        sb.AppendLine();
        
        sb.AppendLine("───────────────────────────────────────────────────────");
        sb.AppendLine("                   CUSTOMER DETAILS                     ");
        sb.AppendLine("───────────────────────────────────────────────────────");
        
        if (quotation.Enquiry != null)
        {
            sb.AppendLine($"Name: {quotation.Enquiry.CustomerName}");
            sb.AppendLine($"Email: {quotation.Enquiry.CustomerEmail ?? "N/A"}");
            sb.AppendLine($"Phone: {quotation.Enquiry.CustomerPhone}");
            sb.AppendLine($"Address: {quotation.Enquiry.CustomerAddress ?? "N/A"}");
        }
        
        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────────");
        sb.AppendLine("                        ITEMS                          ");
        sb.AppendLine("───────────────────────────────────────────────────────");
        
        if (quotation.Items.Any())
        {
            sb.AppendLine(string.Format("{0,-30} {1,-10} {2,-12} {3,-12}", 
                "Material", "Qty", "Unit Cost", "Line Total"));
            sb.AppendLine(new string('-', 64));
            
            foreach (var item in quotation.Items)
            {
                sb.AppendLine(string.Format("{0,-30} {1,-10} ₹{2,-11:F2} ₹{3,-11:F2}", 
                    item.Material?.Name ?? "Unknown Material", 
                    item.Quantity.ToString("F2"), 
                    item.UnitCost, 
                    item.LineTotal));
            }
        }
        
        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine($"Subtotal:                                  ₹{quotation.Subtotal:F2}");
        
        if (quotation.TaxPercentage > 0)
        {
            sb.AppendLine($"Tax ({quotation.TaxPercentage}%):                              ₹{quotation.TaxAmount:F2}");
        }
        
        sb.AppendLine($"TOTAL AMOUNT:                              ₹{quotation.TotalAmount:F2}");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        
        if (!string.IsNullOrWhiteSpace(quotation.Notes))
        {
            sb.AppendLine();
            sb.AppendLine("Notes:");
            sb.AppendLine(quotation.Notes);
        }
        
        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine("═══════════════════════════════════════════════════════");

        // Convert to UTF-8 bytes
        var pdfBytes = Encoding.UTF8.GetBytes(sb.ToString());

        _logger.LogInformation($"PDF generated successfully for quotation: {quotationId}");

        return pdfBytes;
    }

    public async Task<string> SavePdfAsync(byte[] pdfBytes, string fileName)
    {
        _logger.LogInformation($"Saving PDF file: {fileName}");

        // Create unique file name
        var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_pdfDirectory, uniqueFileName);

        // Save file
        await File.WriteAllBytesAsync(filePath, pdfBytes);

        // Return relative path for storage in database
        var relativePath = Path.Combine("pdfs", uniqueFileName).Replace("\\", "/");

        _logger.LogInformation($"PDF saved successfully: {relativePath}");

        return relativePath;
    }
}
