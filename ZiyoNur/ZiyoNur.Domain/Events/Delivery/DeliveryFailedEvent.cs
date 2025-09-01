using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Delivery;

public class DeliveryFailedEvent : BaseEvent
{
    public int DeliveryId { get; }
    public int OrderId { get; }
    public string TrackingCode { get; }
    public string Reason { get; }

    public DeliveryFailedEvent(int deliveryId, int orderId, string trackingCode, string reason)
    {
        DeliveryId = deliveryId;
        OrderId = orderId;
        TrackingCode = trackingCode;
        Reason = reason;
    }
}
