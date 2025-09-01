using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Orders;

public class DiscountReason : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Statistics
    public int UsageCount { get; set; } = 0;
    public decimal TotalDiscountGiven { get; set; } = 0;

    // Navigation Properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    // Business Methods
    public void IncrementUsage(decimal discountAmount)
    {
        UsageCount++;
        TotalDiscountGiven += discountAmount;
    }
}
