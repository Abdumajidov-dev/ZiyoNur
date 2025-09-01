using System.ComponentModel.DataAnnotations;

namespace ZiyoNur.Service.DTOs.Products;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}