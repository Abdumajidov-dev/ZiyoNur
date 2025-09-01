using System.ComponentModel.DataAnnotations;

namespace ZiyoNur.Service.DTOs.Auth;

public class RegisterCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? Address { get; set; }
    public string? FcmToken { get; set; }
}

public class RegisterResponse
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool RequiresVerification { get; set; }
}