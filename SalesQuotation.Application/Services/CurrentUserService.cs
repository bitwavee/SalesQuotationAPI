using Microsoft.AspNetCore.Http;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Extracts the current authenticated user's ID from HttpContext.
/// </summary>
public interface ICurrentUserService
{
    Guid GetUserId();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var sub = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        return sub != null ? Guid.Parse(sub) : Guid.Empty;
    }
}
