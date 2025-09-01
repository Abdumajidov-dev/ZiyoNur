using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Domain.Entities.Products;

public class Cart : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    // Calculated properties
    public decimal TotalPrice => Product?.Price * Quantity ?? 0;

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;

    // Business Methods
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        if (Product != null && !Product.CanOrder(newQuantity))
            throw new InvalidOperationException("Not enough stock available");

        Quantity = newQuantity;
    }

    public void IncrementQuantity(int amount = 1)
    {
        UpdateQuantity(Quantity + amount);
    }

    public void DecrementQuantity(int amount = 1)
    {
        var newQuantity = Quantity - amount;
        if (newQuantity <= 0)
            throw new InvalidOperationException("Cannot decrement quantity below 1");

        UpdateQuantity(newQuantity);
    }
}
