using System.ComponentModel.DataAnnotations;

namespace ZiyoNur.Service.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? FcmToken { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // customer, seller, admin
    public decimal? TotalCashback { get; set; } // Only for customers
}
