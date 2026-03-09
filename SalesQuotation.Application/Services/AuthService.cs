using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SalesQuotation.Domain.Entities;
using SalesQuotation.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IAuthService for handling authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    public async Task<(User user, string token)> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Login attempt with empty email or password");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted && u.IsActive);

        if (user == null)
        {
            _logger.LogWarning("Login failed: no active user found for email {Email}", email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: wrong password for email {Email}", email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = GenerateToken(user);
        _logger.LogInformation("User {Email} logged in successfully", email);

        return (user, token);
    }

    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    public string GenerateToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT secret key is not configured properly");
        }

        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "10080")),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Verifies a plaintext password against a stored hash.
    /// Supports HMAC-SHA256 "key:hash" format used by StaffService.
    /// </summary>
    private bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
            return false;

        // HMAC-SHA256 format: "base64(key):base64(hash)"
        if (storedHash.Contains(':'))
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] keyBytes = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }

        return false;
    }

    /// <summary>
    /// Hashes a password using HMAC-SHA256 (same scheme as StaffService).
    /// Returns "base64(key):base64(hash)".
    /// </summary>
    public static string HashPassword(string password)
    {
        using var hmac = new HMACSHA256();
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hmac.Key) + ":" + Convert.ToBase64String(hash);
    }
}
