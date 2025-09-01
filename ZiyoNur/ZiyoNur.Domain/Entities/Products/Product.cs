using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Products;

public class Product : BaseAuditableEntity, IHasDomainEvent
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Count { get; set; } = 0;

    public int CategoryId { get; set; }

    [MaxLength(100)]
    public string? QrCode { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    // SEO and Search
    [MaxLength(500)]
    public string? SearchKeywords { get; set; }

    [MaxLength(200)]
    public string? Barcode { get; set; }

    // Metrics
    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int SoldCount { get; set; } = 0;
    public decimal AverageRating { get; set; } = 0;

    // Business Properties
    public bool IsAvailable => Status == ProductStatus.Active && Count > 0;
    public bool IsOutOfStock => Count <= 0;
    public string StatusText => Status.ToString();

    // Navigation Properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
    public virtual ICollection<ProductLike> Likes { get; set; } = new List<ProductLike>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool CanOrder(int quantity)
    {
        return IsAvailable && Count >= quantity;
    }

    public void UpdateStock(int quantity, string reason = "")
    {
        var oldCount = Count;
        Count = Math.Max(0, Count + quantity);

        // Update status if out of stock
        if (Count == 0 && Status == ProductStatus.Active)
        {
            Status = ProductStatus.OutOfStock;
        }
        else if (Count > 0 && Status == ProductStatus.OutOfStock)
        {
            Status = ProductStatus.Active;
        }

        // Domain event for stock change
        DomainEvents.Add(new Events.Products.ProductStockUpdatedEvent(
            Id, oldCount, Count, reason));
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void UpdateMetrics()
    {
        LikeCount = Likes.Count;
        SoldCount = OrderItems
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .Sum(oi => oi.Quantity);
    }

    public void SetPrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative");

        var oldPrice = Price;
        Price = newPrice;

        DomainEvents.Add(new Events.Products.ProductPriceChangedEvent(
            Id, oldPrice, newPrice));
    }
}
