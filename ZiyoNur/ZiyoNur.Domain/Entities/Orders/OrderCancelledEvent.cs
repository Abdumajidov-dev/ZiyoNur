using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Orders;

public class OrderCancelledEvent : BaseEvent
{
    public int OrderId { get; }
    public int CustomerId { get; }
    public string Reason { get; }

    public OrderCancelledEvent(int orderId, int customerId, string reason)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Reason = reason;
    }
}
