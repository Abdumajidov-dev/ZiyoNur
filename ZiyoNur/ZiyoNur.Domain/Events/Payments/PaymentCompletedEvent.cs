using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Payments;

public class PaymentCompletedEvent : BaseEvent
{
    public int PaymentId { get; }
    public int OrderId { get; }
    public decimal Amount { get; }
    public string PaymentMethod { get; }

    public PaymentCompletedEvent(int paymentId, int orderId, decimal amount, string paymentMethod)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
    }
}
