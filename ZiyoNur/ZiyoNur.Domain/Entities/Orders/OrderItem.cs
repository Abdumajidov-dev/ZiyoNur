using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Domain.Entities.Orders;

public class OrderItem : BaseAuditableEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal DiscountApplied { get; set; } = 0;
    public decimal TotalPrice { get; set; }

    // Calculated Properties
    public decimal FinalPrice => TotalPrice - DiscountApplied;
    public decimal DiscountPercentage => TotalPrice > 0 ? (DiscountApplied / TotalPrice) * 100 : 0;

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;

    // Business Methods
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        Quantity = newQuantity;
        TotalPrice = UnitPrice * Quantity;
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount < 0 || discountAmount > TotalPrice)
            throw new ArgumentException("Invalid discount amount");

        DiscountApplied = discountAmount;
    }
}
