using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Orders;

public class OrderCreatedEvent : BaseEvent
{
    public int OrderId { get; }
    public int CustomerId { get; }
    public decimal TotalAmount { get; }
    public PaymentMethod PaymentMethod { get; }
    public DeliveryType DeliveryType { get; }

    public OrderCreatedEvent(int orderId, int customerId, decimal totalAmount,
        PaymentMethod paymentMethod, DeliveryType deliveryType)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        DeliveryType = deliveryType;
    }
}
