using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Queries;

public class GetCustomerByIdQuery : BaseRequest<BaseResponse<UserDto>>
{
    public int Id { get; set; }

    public GetCustomerByIdQuery(int id)
    {
        Id = id;
    }
}