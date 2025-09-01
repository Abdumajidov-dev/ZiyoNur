using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Orders;

public class OrderStatusChangedEvent : BaseEvent
{
    public int OrderId { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public int ChangedById { get; }
    public string ChangedByType { get; }

    public OrderStatusChangedEvent(int orderId, OrderStatus oldStatus, OrderStatus newStatus,
        int changedById, string changedByType)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedById = changedById;
        ChangedByType = changedByType;
    }
}
