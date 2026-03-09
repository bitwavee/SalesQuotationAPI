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
        var userRole = context.User.FindFirst("role")?.Value;
        var userId = context.User.FindFirst("sub")?.Value;

        // Skip RBAC for unauthenticated users — let [Authorize] / [AllowAnonymous] handle it
        if (string.IsNullOrEmpty(userRole))
        {
            await _next(context);
            return;
        }

        // Check admin-only endpoints (Staff fully blocked)
        if (IsAdminOnlyEndpoint(path))
        {
            if (userRole != "Admin")
            {
                _logger.LogWarning("Unauthorized access attempt to admin endpoint: {Path} by role: {Role}", path, userRole);
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

        // Staff can read and create on enquiry/measurement/quotation paths
        // Only admin can DELETE enquiries
        if (userRole == "Staff" && path.StartsWith("/api/enquiry") && method == "DELETE")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Access denied. Admin role required for deleting enquiries.",
                code = "FORBIDDEN"
            });
            return;
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
