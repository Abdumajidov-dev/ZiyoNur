using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Products.Queries;

public class GetProductByIdQuery : BaseRequest<BaseResponse<ProductDto>>
{
    public int Id { get; set; }
    public int? CurrentUserId { get; set; }

    public GetProductByIdQuery(int id, int? currentUserId = null)
    {
        Id = id;
        CurrentUserId = currentUserId;
    }
}