using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Payments;

public class PaymentCancelledEvent : BaseEvent
{
    public int PaymentId { get; }
    public int OrderId { get; }
    public string Reason { get; }

    public PaymentCancelledEvent(int paymentId, int orderId, string reason)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Reason = reason;
    }
}
