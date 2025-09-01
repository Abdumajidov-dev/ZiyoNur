using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Delivery;

public class DeliveryCompletedEvent : BaseEvent
{
    public int DeliveryId { get; }
    public int OrderId { get; }
    public string TrackingCode { get; }
    public DateTime DeliveredAt { get; }

    public DeliveryCompletedEvent(int deliveryId, int orderId, string trackingCode, DateTime deliveredAt)
    {
        DeliveryId = deliveryId;
        OrderId = orderId;
        TrackingCode = trackingCode;
        DeliveredAt = deliveredAt;
    }
}
