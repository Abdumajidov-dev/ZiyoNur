using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Delivery;
using ZiyoNur.Domain.Entities.Payments;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Orders;

public class Order : BaseAuditableEntity, IHasDomainEvent
{
    public int CustomerId { get; set; }
    public int? SellerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }
    public decimal DiscountApplied { get; set; } = 0;
    public int? DiscountReasonId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DeliveryType DeliveryType { get; set; }

    [MaxLength(100)]
    public string PickupLocation { get; set; } = "main_library";

    [MaxLength(20)]
    public string OrderSource { get; set; } = "mobile"; // mobile, pos_system

    public string? Notes { get; set; }

    // Calculated Properties
    public decimal FinalPrice => TotalPrice - DiscountApplied;
    public bool IsOnlineOrder => !SellerId.HasValue;
    public bool IsCompleted => Status == OrderStatus.Delivered;
    public bool CanBeCancelled => Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Seller? Seller { get; set; }
    public virtual DiscountReason? DiscountReason { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; } = new List<CashbackTransaction>();
    public virtual OrderDelivery? Delivery { get; set; }

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public void AddItem(Product product, int quantity, decimal? customPrice = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        if (!product.CanOrder(quantity))
            throw new InvalidOperationException($"Not enough stock for {product.Name}");

        var existingItem = OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var unitPrice = customPrice ?? product.Price;
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = unitPrice * quantity
            };
            OrderItems.Add(orderItem);
        }

        RecalculateTotalPrice();
    }

    public void RemoveItem(int productId)
    {
        var item = OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
        if (item != null)
        {
            OrderItems.Remove(item);
            RecalculateTotalPrice();
        }
    }

    public void ApplyDiscount(decimal discountAmount, int? reasonId = null)
    {
        if (discountAmount < 0 || discountAmount > TotalPrice)
            throw new ArgumentException("Invalid discount amount");

        DiscountApplied = discountAmount;
        DiscountReasonId = reasonId;

        // Apply discount to items (starting from cheapest)
        ApplyDiscountToItems(discountAmount);
    }

    public void ChangeStatus(OrderStatus newStatus, int changedById, string changedByType, string? notes = null)
    {
        var oldStatus = Status;
        Status = newStatus;

        // Add to history
        StatusHistory.Add(new OrderStatusHistory
        {
            OrderId = Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedBy = changedByType,
            ChangedById = changedById,
            Notes = notes
        });

        // Domain event
        DomainEvents.Add(new Events.Orders.OrderStatusChangedEvent(
            Id, oldStatus, newStatus, changedById, changedByType));

        // Special logic for completed orders
        if (newStatus == OrderStatus.Delivered && oldStatus != OrderStatus.Delivered)
        {
            ProcessCashback();
            UpdateProductSalesCount();
        }
    }

    public void Cancel(int cancelledById, string cancelledByType, string reason)
    {
        if (!CanBeCancelled)
            throw new InvalidOperationException("Order cannot be cancelled");

        ChangeStatus(OrderStatus.Cancelled, cancelledById, cancelledByType, reason);

        // Restore product stock
        foreach (var item in OrderItems)
        {
            item.Product.UpdateStock(item.Quantity, "Order cancelled");
        }

        DomainEvents.Add(new Events.Orders.OrderCancelledEvent(
            Id, CustomerId, reason));
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = OrderItems.Sum(item => item.TotalPrice);
    }

    private void ApplyDiscountToItems(decimal totalDiscount)
    {
        var remainingDiscount = totalDiscount;
        var sortedItems = OrderItems.OrderBy(item => item.UnitPrice).ToList();

        foreach (var item in sortedItems)
        {
            if (remainingDiscount <= 0) break;

            var itemTotal = item.TotalPrice;
            var itemDiscount = Math.Min(remainingDiscount, itemTotal);

            item.DiscountApplied = itemDiscount;
            remainingDiscount -= itemDiscount;
        }
    }

    private void ProcessCashback()
    {
        // Calculate 2% cashback
        var cashbackAmount = FinalPrice * 0.02m;
        if (cashbackAmount > 0)
        {
            CashbackTransactions.Add(new CashbackTransaction
            {
                CustomerId = CustomerId,
                OrderId = Id,
                Amount = cashbackAmount,
                Type = CashbackTransactionType.Earned,
                ExpiryDate = DateTime.UtcNow.AddDays(30) // 30 days validity
            });

            DomainEvents.Add(new Events.Orders.CashbackEarnedEvent(
                CustomerId, cashbackAmount, Id));
        }
    }

    private void UpdateProductSalesCount()
    {
        foreach (var item in OrderItems)
        {
            item.Product.SoldCount += item.Quantity;
        }
    }
}
