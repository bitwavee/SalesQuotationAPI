using Microsoft.EntityFrameworkCore;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Domain.Enums;

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
            entity.Property(e => e.Role).HasConversion<string>();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Material configuration
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedMaterials).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
        });

        // Enquiry configuration
        modelBuilder.Entity<Enquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnquiryNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.EnquiryNumber).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedEnquiries).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.AssignedStaff).WithMany(u => u.AssignedEnquiries).HasForeignKey(e => e.AssignedStaffId).OnDelete(DeleteBehavior.NoAction);
            // Status is a plain string column — no FK to EnquiryStatusConfig
            entity.Ignore(e => e.StatusConfig);
        });

        // Quotation configuration
        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuotationNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.QuotationNumber).IsUnique();
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Quotations).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedQuotations).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
        });

        // QuotationItem configuration
        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quotation).WithMany(q => q.Items).HasForeignKey(e => e.QuotationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Material).WithMany(m => m.QuotationItems).HasForeignKey(e => e.MaterialId).OnDelete(DeleteBehavior.NoAction);
        });

        // Measurement configuration
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Measurements).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category).WithMany(mc => mc.Measurements).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
        });

        // EnquiryStatusConfig configuration
        modelBuilder.Entity<EnquiryStatusConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StatusValue).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.StatusValue).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
            entity.Ignore(e => e.Enquiries);
        });

        // EnquiryProgress configuration
        modelBuilder.Entity<EnquiryProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.ProgressHistory).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
        });

        // FileUpload configuration
        // Enquiry → Quotation is Cascade, so both Enquiry → FileUpload AND
        // Quotation → FileUpload cannot cascade/setNull — SQL Server rejects
        // multiple paths. Use NoAction on all three FKs; handle orphan cleanup in code.
        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enquiry).WithMany(eq => eq.Attachments).HasForeignKey(e => e.EnquiryId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Quotation).WithMany(q => q.Attachments).HasForeignKey(e => e.QuotationId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.UploadedBy).WithMany().HasForeignKey(e => e.UploadedById).OnDelete(DeleteBehavior.NoAction);
        });

        // MeasurementCategory configuration
        modelBuilder.Entity<MeasurementCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CategoryKey).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CategoryKey).IsUnique();
            entity.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.NoAction);
        });

        // Decimal precision for monetary / quantity fields
        modelBuilder.Entity<Material>()
            .Property(e => e.BaseCost).HasPrecision(18, 2);

        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxPercentage).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.UnitCost).HasPrecision(18, 2);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Measurement>()
            .Property(e => e.CalculatedValue).HasPrecision(18, 4);
    }
}