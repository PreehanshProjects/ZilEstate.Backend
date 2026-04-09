using System.ComponentModel.DataAnnotations;

namespace ZilEstate.Application.DTOs;

public class RegisterDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public bool IsAgency { get; set; }
    public string? AgencyName { get; set; }
    public string? AgencyDescription { get; set; }
    public string? AgencyLogoUrl { get; set; }
    public string? AgencyWebsite { get; set; }
    public string? AgencyPhone { get; set; }
    public string? AgencyEmail { get; set; }
    public string? AgencyAddress { get; set; }
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? AgencyId { get; set; }
    public string Token { get; set; } = string.Empty;
}
