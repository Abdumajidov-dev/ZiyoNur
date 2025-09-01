using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Orders;

public class CashbackEarnedEvent : BaseEvent
{
    public int CustomerId { get; }
    public decimal Amount { get; }
    public int OrderId { get; }

    public CashbackEarnedEvent(int customerId, decimal amount, int orderId)
    {
        CustomerId = customerId;
        Amount = amount;
        OrderId = orderId;
    }
}
