using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Products;

public class Category : BaseAuditableEntity, IHasDomainEvent
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int SortOrder { get; set; } = 0;

    // Navigation Properties
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool IsSubCategory => ParentId.HasValue;
    public bool HasProducts => Products.Any();
    public bool HasSubCategories => SubCategories.Any();

    public string GetFullPath()
    {
        if (Parent == null) return Name;
        return $"{Parent.GetFullPath()} > {Name}";
    }

    public void AddSubCategory(string name, string? description = null)
    {
        SubCategories.Add(new Category
        {
            Name = name,
            Description = description,
            ParentId = Id,
            IsActive = true
        });
    }
}
