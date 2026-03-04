// This file is deprecated. Use SalesQuotation.Application.Dtos instead.
// Keep this file for backwards compatibility but do not add new code here.

namespace SalesQuotation.Infrastructure.Dtos;

// DTOs have been moved to SalesQuotation.Application.Dtos.AllDtos


public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public UserDto User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "STAFF";
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
}