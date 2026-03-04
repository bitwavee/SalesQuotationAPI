using SalesQuotation.Domain.Entities;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="password">User password</param>
    /// <returns>Tuple containing authenticated user and JWT token</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid</exception>
    Task<(User user, string token)> LoginAsync(string email, string password);

    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="user">User entity</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(User user);
}
