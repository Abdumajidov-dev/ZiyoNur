using System.ComponentModel.DataAnnotations;

namespace ZiyoNur.Service.DTOs.Auth;

public class CreateSellerRequest
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

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Role { get; set; } = "seller";
}

public class UpdateSellerRequest
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

    [MaxLength(50)]
    public string Role { get; set; } = "seller";

    public bool IsActive { get; set; } = true;
}