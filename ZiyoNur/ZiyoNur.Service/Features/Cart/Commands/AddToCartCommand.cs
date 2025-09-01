using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Features.Cart.Commands;

public class AddToCartCommand : BaseRequest<BaseResponse>
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;

    public AddToCartCommand(int customerId, int productId, int quantity = 1)
    {
        CustomerId = customerId;
        ProductId = productId;
        Quantity = quantity;
    }
}