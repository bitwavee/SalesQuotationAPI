using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IPdfService for PDF generation using QuestPDF
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

        if (!Directory.Exists(_pdfDirectory))
        {
            Directory.CreateDirectory(_pdfDirectory);
        }

        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateQuotationPdfAsync(Guid quotationId)
    {
        _logger.LogInformation("Generating PDF for quotation: {QuotationId}", quotationId);

        var quotation = await _context.Quotations
            .Include(q => q.Enquiry)
            .Include(q => q.Items)
            .ThenInclude(i => i.Material)
            .FirstOrDefaultAsync(q => q.Id == quotationId);

        if (quotation == null)
        {
            throw new KeyNotFoundException($"Quotation not found: {quotationId}");
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("QUOTATION").Bold().FontSize(20).AlignCenter();
                    col.Item().PaddingTop(5).LineHorizontal(1);
                });

                page.Content().PaddingTop(15).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text($"Quotation #: {quotation.QuotationNumber}").Bold();
                            left.Item().Text($"Date: {quotation.QuotationDate:yyyy-MM-dd}");
                            left.Item().Text($"Valid Until: {quotation.ValidUntil:yyyy-MM-dd}");
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                    if (quotation.Enquiry != null)
                    {
                        col.Item().Text("Customer Details").Bold().FontSize(12);
                        col.Item().PaddingTop(3).Text($"Name: {quotation.Enquiry.CustomerName}");
                        col.Item().Text($"Email: {quotation.Enquiry.CustomerEmail ?? "N/A"}");
                        col.Item().Text($"Phone: {quotation.Enquiry.CustomerPhone}");
                        col.Item().Text($"Address: {quotation.Enquiry.CustomerAddress ?? "N/A"}");
                    }

                    col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                    col.Item().Text("Items").Bold().FontSize(12);
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Material").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qty").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unit Cost").Bold().AlignRight();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").Bold().AlignRight();
                        });

                        if (quotation.Items.Any())
                        {
                            foreach (var item in quotation.Items)
                            {
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text(item.Material?.Name ?? "Unknown");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text(item.Quantity.ToString("F2"));
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text($"\u20b9{item.UnitCost:F2}").AlignRight();
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text($"\u20b9{item.LineTotal:F2}").AlignRight();
                            }
                        }
                        else
                        {
                            table.Cell().ColumnSpan(4).Padding(10).Text("No items").AlignCenter();
                        }
                    });

                    col.Item().PaddingTop(15).AlignRight().Column(totals =>
                    {
                        totals.Item().Text($"Subtotal: \u20b9{quotation.Subtotal:F2}");
                        if (quotation.TaxPercentage > 0)
                        {
                            totals.Item().Text($"Tax ({quotation.TaxPercentage}%): \u20b9{quotation.TaxAmount:F2}");
                        }
                        totals.Item().Text($"Total Amount: \u20b9{quotation.TotalAmount:F2}").Bold().FontSize(12);
                    });

                    if (!string.IsNullOrWhiteSpace(quotation.Notes))
                    {
                        col.Item().PaddingTop(15).Text("Notes:").Bold();
                        col.Item().Text(quotation.Notes);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC | Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });

        var pdfBytes = document.GeneratePdf();

        _logger.LogInformation("PDF generated successfully for quotation: {QuotationId}", quotationId);

        return pdfBytes;
    }

    public async Task<string> SavePdfAsync(byte[] pdfBytes, string fileName)
    {
        _logger.LogInformation("Saving PDF file: {FileName}", fileName);

        var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_pdfDirectory, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var relativePath = Path.Combine("pdfs", uniqueFileName).Replace("\\", "/");

        _logger.LogInformation("PDF saved successfully: {Path}", relativePath);

        return relativePath;
    }
}
