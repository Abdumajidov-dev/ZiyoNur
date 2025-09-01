using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Payments;

public class PaymentFailedEvent : BaseEvent
{
    public int PaymentId { get; }
    public int OrderId { get; }
    public decimal Amount { get; }
    public string Reason { get; }

    public PaymentFailedEvent(int paymentId, int orderId, decimal amount, string reason)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        Reason = reason;
    }
}
