using Microsoft.EntityFrameworkCore;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Domain.Enums;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;

namespace SalesQuotation.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<EnquiryStatusConfig> EnquiryStatusConfigs { get; set; }
    public DbSet<Enquiry> Enquiries { get; set; }
    public DbSet<MeasurementCategory> MeasurementCategories { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<QuotationItem> QuotationItems { get; set; }
    public DbSet<EnquiryProgress> EnquiryProgress { get; set; }
    public DbSet<FileUpload> FileUploads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<string>();
        });

        // Material configuration
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedMaterials).HasForeignKey(e => e.CreatedById);
        });

        // Enquiry configuration
        modelBuilder.Entity<Enquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnquiryNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.EnquiryNumber).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedEnquiries).HasForeignKey(e => e.CreatedById);
            entity.HasOne(e => e.AssignedStaff).WithMany(u => u.AssignedEnquiries).HasForeignKey(e => e.AssignedStaffId);
            entity.HasOne(e => e.StatusConfig).WithMany(s => s.Enquiries).HasForeignKey(e => e.Status).HasPrincipalKey(s => s.StatusValue);
        });

        // Quotation configuration
        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuotationNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.QuotationNumber).IsUnique();
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Quotations).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedQuotations).HasForeignKey(e => e.CreatedById);
        });

        // QuotationItem configuration
        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quotation).WithMany(q => q.Items).HasForeignKey(e => e.QuotationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Material).WithMany(m => m.QuotationItems).HasForeignKey(e => e.MaterialId);
        });

        // Measurement configuration
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Measurements).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category).WithMany(mc => mc.Measurements).HasForeignKey(e => e.CategoryId);
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
        });

        // EnquiryStatusConfig configuration
        modelBuilder.Entity<EnquiryStatusConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StatusValue).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.StatusValue).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
        });

        // EnquiryProgress configuration
        modelBuilder.Entity<EnquiryProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.ProgressHistory).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
        });

        // FileUpload configuration
        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Attachments).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Quotation).WithMany(q => q.Attachments).HasForeignKey(e => e.QuotationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.UploadedBy).WithMany().HasForeignKey(e => e.UploadedById);
        });

        // MeasurementCategory configuration
        modelBuilder.Entity<MeasurementCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CategoryKey).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CategoryKey).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
        });

        // Seed default status configurations
        SeedEnquiryStatuses(modelBuilder);
    }

    private void SeedEnquiryStatuses(ModelBuilder modelBuilder)
    {
        var adminId = Guid.NewGuid();

        modelBuilder.Entity<EnquiryStatusConfig>().HasData(
            new EnquiryStatusConfig
            {
                Id = Guid.NewGuid(),
                StatusName = "Initiated",
                StatusValue = "INITIATED",
                DisplayOrder = 1,
                ColorHex = "#0099FF",
                IsActive = true,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new EnquiryStatusConfig
            {
                Id = Guid.NewGuid(),
                StatusName = "Site Visited",
                StatusValue = "SITE_VISITED",
                DisplayOrder = 2,
                ColorHex = "#FFA500",
                IsActive = true,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new EnquiryStatusConfig
            {
                Id = Guid.NewGuid(),
                StatusName = "Quotation Sent",
                StatusValue = "QUOTATION_SENT",
                DisplayOrder = 3,
                ColorHex = "#4CAF50",
                IsActive = true,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new EnquiryStatusConfig
            {
                Id = Guid.NewGuid(),
                StatusName = "Follow-up",
                StatusValue = "FOLLOW_UP",
                DisplayOrder = 4,
                ColorHex = "#9C27B0",
                IsActive = true,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new EnquiryStatusConfig
            {
                Id = Guid.NewGuid(),
                StatusName = "Closed",
                StatusValue = "CLOSED",
                DisplayOrder = 5,
                ColorHex = "#808080",
                IsActive = true,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}