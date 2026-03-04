using SalesQuotation.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for file upload and management
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Upload file
    /// </summary>
    Task<FileUploadDto> UploadAsync(IFormFile file, Guid enquiryId, string category = "ATTACHMENT");

    /// <summary>
    /// Get file by ID
    /// </summary>
    Task<FileUploadDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all files for enquiry
    /// </summary>
    Task<IEnumerable<FileUploadDto>> GetEnquiryFilesAsync(Guid enquiryId);

    /// <summary>
    /// Delete file
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Get file content
    /// </summary>
    Task<byte[]> GetFileContentAsync(Guid id);
}
