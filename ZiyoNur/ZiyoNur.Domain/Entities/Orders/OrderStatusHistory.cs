using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Orders;

public class OrderStatusHistory : BaseAuditableEntity
{
    public int OrderId { get; set; }

    public OrderStatus OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }

    [MaxLength(20)]
    public string ChangedBy { get; set; } = string.Empty; // admin, seller, system

    public int ChangedById { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
}
