using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Orders;

namespace ZiyoNur.Service.Features.Products.Commands;

public class CreateOrderCommand : BaseRequest<BaseResponse<OrderDto>>
{
    public int CustomerId { get; set; }
    public int? SellerId { get; set; } // Null if online order
    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty;
    public string? DeliveryAddress { get; set; }
    public decimal CashbackToUse { get; set; }
    public string? Notes { get; set; }
    public string OrderSource { get; set; } = "mobile"; // mobile, pos_system

    public CreateOrderCommand(CreateOrderRequest request, int customerId, int? sellerId = null)
    {
        CustomerId = customerId;
        SellerId = sellerId;
        Items = request.Items;
        PaymentMethod = request.PaymentMethod;
        DeliveryType = request.DeliveryType;
        DeliveryAddress = request.DeliveryAddress;
        CashbackToUse = request.CashbackToUse;
        Notes = request.Notes;
        OrderSource = sellerId.HasValue ? "pos_system" : "mobile";
    }
}