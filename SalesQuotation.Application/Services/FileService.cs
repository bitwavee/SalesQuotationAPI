using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IFileService for file uploads
/// </summary>
public class FileService : IFileService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadDirectory;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public FileService(ApplicationDbContext context, ICurrentUserService currentUser, ILogger<FileService> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
        _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<FileUploadDto> UploadAsync(IFormFile file, Guid enquiryId, string category = "ATTACHMENT")
    {
        _logger.LogInformation($"Uploading file: {file.FileName} for enquiry: {enquiryId}");

        // Validate enquiry exists
        var enquiry = await _context.Enquiries.FindAsync(enquiryId);
        if (enquiry == null)
        {
            throw new KeyNotFoundException($"Enquiry not found: {enquiryId}");
        }

        // Validate file
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds maximum limit of {MaxFileSize / (1024 * 1024)} MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Any(e => e == extension))
        {
            throw new ArgumentException($"File type not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        // Generate unique file name
        var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save to database
        var fileUpload = new FileUpload
        {
            Id = Guid.NewGuid(),
            EnquiryId = enquiryId,
            FileName = file.FileName,
            FileType = file.ContentType,
            FileSizeBytes = (int)file.Length,
            FilePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/"),
            UploadedById = _currentUser.GetUserId(),
            CreatedAt = DateTime.UtcNow
        };

        _context.FileUploads.Add(fileUpload);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"File uploaded successfully: {fileUpload.Id}");

        return new FileUploadDto
        {
            Id = fileUpload.Id,
            EnquiryId = fileUpload.EnquiryId ?? Guid.Empty,
            FileName = fileUpload.FileName,
            FileType = fileUpload.FileType ?? "",
            FileSize = fileUpload.FileSizeBytes ?? 0,
            FilePath = fileUpload.FilePath,
            Category = "ATTACHMENT",
            UploadedAt = fileUpload.CreatedAt
        };
    }

    public async Task<FileUploadDto?> GetByIdAsync(Guid id)
    {
        var file = await _context.FileUploads.FindAsync(id);

        if (file == null)
            return null;

        return new FileUploadDto
        {
            Id = file.Id,
            EnquiryId = file.EnquiryId ?? Guid.Empty,
            FileName = file.FileName,
            FileType = file.FileType ?? "",
            FileSize = file.FileSizeBytes ?? 0,
            FilePath = file.FilePath,
            Category = "ATTACHMENT",
            UploadedAt = file.CreatedAt
        };
    }

    public async Task<IEnumerable<FileUploadDto>> GetEnquiryFilesAsync(Guid enquiryId)
    {
        var files = await _context.FileUploads
            .Where(f => f.EnquiryId == enquiryId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return files.Select(f => new FileUploadDto
        {
            Id = f.Id,
            EnquiryId = f.EnquiryId ?? Guid.Empty,
            FileName = f.FileName,
            FileType = f.FileType ?? "",
            FileSize = f.FileSizeBytes ?? 0,
            FilePath = f.FilePath,
            Category = "ATTACHMENT",
            UploadedAt = f.CreatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"Deleting file: {id}");

        var file = await _context.FileUploads.FindAsync(id);

        if (file == null)
        {
            throw new KeyNotFoundException($"File not found: {id}");
        }

        // Delete physical file
        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath);
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        // Delete from database
        _context.FileUploads.Remove(file);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"File deleted successfully: {id}");
    }

    public async Task<byte[]> GetFileContentAsync(Guid id)
    {
        _logger.LogInformation($"Reading file content: {id}");

        var file = await _context.FileUploads.FindAsync(id);

        if (file == null)
        {
            throw new KeyNotFoundException($"File not found: {id}");
        }

        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath);

        if (!File.Exists(physicalPath))
        {
            throw new FileNotFoundException($"Physical file not found: {physicalPath}");
        }

        return await File.ReadAllBytesAsync(physicalPath);
    }
}
