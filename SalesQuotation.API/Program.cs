using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesQuotation.API;
using SalesQuotation.API.Middleware;
using SalesQuotation.Application.Services;
using SalesQuotation.Application.Validators;
using SalesQuotation.Infrastructure.Data;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== LOGGING (Serilog) ==========
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ========== DATABASE ==========
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ========== AUTHENTICATION (JWT) ==========
var secretKey = builder.Configuration["JwtSettings:SecretKey"];

if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT secret key must be configured in appsettings and must be at least 32 characters long");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false; // Keep "sub", "role" etc. as-is — don't remap to long URIs
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = "role",   // [Authorize(Roles="Admin")] reads from "role"
        NameClaimType = "unique_name"
    };
});

// ========== SERVICES ==========
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnquiryService, EnquiryService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IEnquiryStatusConfigService, EnquiryStatusConfigService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMeasurementConversionService, MeasurementConversionService>();
builder.Services.AddScoped<IReportService, ReportService>();

// ========== VALIDATION ==========
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// ========== AUTO MAPPER ==========
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<SalesQuotation.Application.MappingProfile>());

// ========== CONTROLLERS & CORS ==========
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ========== SWAGGER ==========
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    {
        Title = "Sales & Quotation API",
        Version = "v1",
        Description = "REST API for field sales and quotation management with .NET 10"
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ========== MIDDLEWARE ==========
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales & Quotation API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseRoleBasedAccessControl();
app.MapControllers();
app.UseStaticFiles(); // For serving PDFs and uploaded files

// ========== DATABASE & SEED ==========
await DatabaseSeeder.SeedAsync(app.Services);

app.Run();