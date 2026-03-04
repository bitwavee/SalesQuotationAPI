using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Application.Services;

namespace SalesQuotation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var (user, token) = await _authService.LoginAsync(request.Email, request.Password);

            var response = new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Data = new LoginResponseDto
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role.ToString(),
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    },
                    Token = token
                }
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Login attempt failed due to invalid credentials");
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Error = "Invalid credentials",
                Code = "UNAUTHORIZED"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during login");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = "An error occurred during authentication",
                Code = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Logs out the user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public ActionResult<ApiResponse<object>> Logout()
    {
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new { Message = "Logged out successfully" }
        });
    }
}