using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Products;

public class ProductOutOfStockEvent : BaseEvent
{
    public int ProductId { get; }
    public string ProductName { get; }

    public ProductOutOfStockEvent(int productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}
