namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for PDF generation
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Generate PDF quotation
    /// </summary>
    /// <param name="quotationId">Quotation ID</param>
    /// <returns>PDF file bytes</returns>
    Task<byte[]> GenerateQuotationPdfAsync(Guid quotationId);

    /// <summary>
    /// Save PDF to file system and return path
    /// </summary>
    Task<string> SavePdfAsync(byte[] pdfBytes, string fileName);
}
