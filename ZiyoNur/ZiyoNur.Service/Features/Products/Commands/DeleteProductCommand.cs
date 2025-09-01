using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Features.Products.Commands;

public class DeleteProductCommand : BaseRequest<BaseResponse>
{
    public int Id { get; set; }
    public int DeletedById { get; set; }

    public DeleteProductCommand(int id, int deletedById)
    {
        Id = id;
        DeletedById = deletedById;
    }
}