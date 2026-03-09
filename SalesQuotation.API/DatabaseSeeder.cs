using Microsoft.EntityFrameworkCore;
using SalesQuotation.Application.Services;
using SalesQuotation.Domain.Enums;
using SalesQuotation.Infrastructure.Data;

namespace SalesQuotation.API;

/// <summary>
/// Seeds the database with initial data on application startup.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Ensures the admin user exists with a proper password hash.
    /// Call once from Program.cs after building the app.
    /// Default admin credentials: admin@salesquotation.com / Admin@123
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is up-to-date with latest migrations
        await context.Database.MigrateAsync();

        // Seed admin user if none exists
        var adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin && !u.IsDeleted);
        if (!adminExists)
        {
            var admin = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                Email = "admin@salesquotation.com",
                Phone = "1234567890",
                PasswordHash = AuthService.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seed] Admin user created: admin@salesquotation.com / Admin@123");
        }
        else
        {
            // Fix existing admin row if it has the old placeholder hash
            var admin = await context.Users
                .FirstOrDefaultAsync(u => u.Role == UserRole.Admin && !u.IsDeleted);

            if (admin != null && !admin.PasswordHash.Contains(':'))
            {
                // Old hash format — replace with proper HMAC hash
                admin.PasswordHash = AuthService.HashPassword("Admin@123");
                admin.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
                Console.WriteLine($"[Seed] Admin password hash updated to proper HMAC format");
            }
        }

        // Seed default measurement categories if none exist
        if (!await context.MeasurementCategories.AnyAsync())
        {
            var adminUser = await context.Users.FirstAsync(u => u.Role == UserRole.Admin);

            context.MeasurementCategories.AddRange(
                new Domain.Entities.MeasurementCategory
                {
                    Id = Guid.NewGuid(),
                    CategoryName = "Area",
                    CategoryKey = "area",
                    MeasurementFields = "[\"length\",\"width\"]",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.MeasurementCategory
                {
                    Id = Guid.NewGuid(),
                    CategoryName = "Length",
                    CategoryKey = "length",
                    MeasurementFields = "[\"value\"]",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.MeasurementCategory
                {
                    Id = Guid.NewGuid(),
                    CategoryName = "Volume",
                    CategoryKey = "volume",
                    MeasurementFields = "[\"length\",\"width\",\"height\"]",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("[Seed] Default measurement categories created");
        }

        // Seed default enquiry status configs if none exist
        if (!await context.EnquiryStatusConfigs.AnyAsync())
        {
            var adminUser = await context.Users.FirstAsync(u => u.Role == UserRole.Admin);

            context.EnquiryStatusConfigs.AddRange(
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Initiated",
                    StatusValue = "INITIATED",
                    DisplayOrder = 1,
                    ColorHex = "#3498db",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Site Visited",
                    StatusValue = "SITE_VISITED",
                    DisplayOrder = 2,
                    ColorHex = "#f39c12",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Measurement Done",
                    StatusValue = "MEASUREMENT_DONE",
                    DisplayOrder = 3,
                    ColorHex = "#9b59b6",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Quotation Sent",
                    StatusValue = "QUOTATION_SENT",
                    DisplayOrder = 4,
                    ColorHex = "#2ecc71",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Closed Won",
                    StatusValue = "CLOSED_WON",
                    DisplayOrder = 5,
                    ColorHex = "#27ae60",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Domain.Entities.EnquiryStatusConfig
                {
                    Id = Guid.NewGuid(),
                    StatusName = "Closed Lost",
                    StatusValue = "CLOSED_LOST",
                    DisplayOrder = 6,
                    ColorHex = "#e74c3c",
                    IsActive = true,
                    CreatedById = adminUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("[Seed] Default enquiry status configs created");
        }
    }
}
