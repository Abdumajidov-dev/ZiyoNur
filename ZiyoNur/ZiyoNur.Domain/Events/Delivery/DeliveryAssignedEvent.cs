using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Delivery;

public class DeliveryAssignedEvent : BaseEvent
{
    public int DeliveryId { get; }
    public int OrderId { get; }
    public int DeliveryPartnerId { get; }
    public string TrackingCode { get; }

    public DeliveryAssignedEvent(int deliveryId, int orderId, int deliveryPartnerId, string trackingCode)
    {
        DeliveryId = deliveryId;
        OrderId = orderId;
        DeliveryPartnerId = deliveryPartnerId;
        TrackingCode = trackingCode;
    }
}
