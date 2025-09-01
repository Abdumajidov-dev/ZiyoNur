using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Features.Users.Commands;

public class DeleteCustomerCommand : BaseRequest<BaseResponse>
{
    public int Id { get; set; }
    public int DeletedById { get; set; }

    public DeleteCustomerCommand(int id, int deletedById)
    {
        Id = id;
        DeletedById = deletedById;
    }
}
