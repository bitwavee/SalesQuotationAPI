using System.Security.Claims;

namespace SalesQuotation.API.Middleware;

/// <summary>
/// Middleware for Role-Based Access Control (RBAC)
/// Restricts API endpoints based on user roles
/// </summary>
public class RoleBasedAccessControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RoleBasedAccessControlMiddleware> _logger;

    // Admin-only endpoints
    private static readonly string[] AdminOnlyPaths = new[]
    {
        "/api/staff",
        "/api/material",
        "/api/enquirystatusconfig"
    };

    // Staff can only access their own enquiries
    private static readonly string[] OwnEnquiryOnlyPaths = new[]
    {
        "/api/enquiry",
        "/api/measurement",
        "/api/quotation"
    };

    public RoleBasedAccessControlMiddleware(RequestDelegate next, ILogger<RoleBasedAccessControlMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        var method = context.Request.Method;
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = context.User.FindFirst("sub")?.Value;

        // Check admin-only endpoints
        if (IsAdminOnlyEndpoint(path))
        {
            if (userRole != "Admin")
            {
                _logger.LogWarning($"Unauthorized access attempt to admin endpoint: {path} by user role: {userRole}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Access denied. Admin role required.",
                    code = "FORBIDDEN"
                });
                return;
            }
        }

        // Check POST/PUT/DELETE operations on restricted endpoints
        if (IsRestrictedModificationEndpoint(path) && IsModificationMethod(method))
        {
            if (userRole != "Admin")
            {
                _logger.LogWarning($"Unauthorized modification attempt: {method} {path} by user role: {userRole}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Access denied. Admin role required for modifications.",
                    code = "FORBIDDEN"
                });
                return;
            }
        }

        // For staff, restrict to their own enquiries
        if (userRole == "Staff" && IsOwnEnquiryOnlyEndpoint(path))
        {
            // Extract enquiry ID from path (if present)
            if (IsEnquirySpecificOperation(path))
            {
                // This check should be done in the service layer for better control
                // Here we just log the access
                _logger.LogInformation($"Staff access to enquiry data: {path}");
            }
        }

        await _next(context);
    }

    private bool IsAdminOnlyEndpoint(string path)
    {
        return AdminOnlyPaths.Any(adminPath => path.StartsWith(adminPath));
    }

    private bool IsRestrictedModificationEndpoint(string path)
    {
        return OwnEnquiryOnlyPaths.Any(restrictedPath => path.StartsWith(restrictedPath));
    }

    private bool IsOwnEnquiryOnlyEndpoint(string path)
    {
        return OwnEnquiryOnlyPaths.Any(ownPath => path.StartsWith(ownPath));
    }

    private bool IsEnquirySpecificOperation(string path)
    {
        // Check if path has an enquiry ID parameter
        return path.Contains("/enquiry/") || path.Contains("enquiry?");
    }

    private bool IsModificationMethod(string method)
    {
        return method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH";
    }
}

/// <summary>
/// Extension methods for RBAC middleware
/// </summary>
public static class RoleBasedAccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleBasedAccessControl(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RoleBasedAccessControlMiddleware>();
    }
}
