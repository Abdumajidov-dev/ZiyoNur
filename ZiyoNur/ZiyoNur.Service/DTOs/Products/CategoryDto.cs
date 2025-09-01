using ZiyoNur.Service.DTOs.Common;

namespace ZiyoNur.Service.DTOs.Products;

public class CategoryDto : AuditableDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();
    public bool HasSubCategories => SubCategories.Any();
    public bool IsSubCategory => ParentId.HasValue;
}