using System.ComponentModel.DataAnnotations;
using ZiyoNur.Service.DTOs.Common;

namespace ZiyoNur.Service.DTOs.Orders;

public class OrderDto : AuditableDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? SellerId { get; set; }
    public string? SellerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal FinalPrice { get; set; }
    public string? DiscountReason { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty;
    public string PickupLocation { get; set; } = string.Empty;
    public string OrderSource { get; set; } = string.Empty;
    public bool IsOnlineOrder { get; set; }
    public bool CanBeCancelled { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
    public OrderDeliveryDto? Delivery { get; set; }
    public List<PaymentTransactionDto> PaymentTransactions { get; set; } = new();
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal FinalPrice { get; set; }
}

public class OrderDeliveryDto
{
    public string? TrackingCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string DeliveryPartnerName { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
}

public class PaymentTransactionDto
{
    public int Id { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class CreateOrderRequest
{
    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new();

    [Required]
    public string PaymentMethod { get; set; } = string.Empty; // payme, uzcard, cash, cashback

    [Required]
    public string DeliveryType { get; set; } = string.Empty; // pickup, delivery

    public string? DeliveryAddress { get; set; }
    public decimal CashbackToUse { get; set; } = 0;
    public string? Notes { get; set; }
}

public class CreateOrderItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
