using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Products;

public class ProductStockUpdatedEvent : BaseEvent
{
    public int ProductId { get; }
    public int OldCount { get; }
    public int NewCount { get; }
    public string Reason { get; }

    public ProductStockUpdatedEvent(int productId, int oldCount, int newCount, string reason)
    {
        ProductId = productId;
        OldCount = oldCount;
        NewCount = newCount;
        Reason = reason;
    }
}
