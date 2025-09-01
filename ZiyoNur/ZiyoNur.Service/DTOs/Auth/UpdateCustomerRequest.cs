using System.ComponentModel.DataAnnotations;

namespace ZiyoNur.Service.DTOs.Auth;

public class UpdateCustomerRequest
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

    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}