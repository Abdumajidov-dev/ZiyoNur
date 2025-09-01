using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Delivery;

public class DeliveryPickedUpEvent : BaseEvent
{
    public int DeliveryId { get; }
    public int OrderId { get; }
    public string TrackingCode { get; }

    public DeliveryPickedUpEvent(int deliveryId, int orderId, string trackingCode)
    {
        DeliveryId = deliveryId;
        OrderId = orderId;
        TrackingCode = trackingCode;
    }
}
