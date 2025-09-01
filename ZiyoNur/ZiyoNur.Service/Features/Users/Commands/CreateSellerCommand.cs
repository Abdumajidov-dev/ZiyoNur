using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Commands;

public class CreateSellerCommand : BaseRequest<BaseResponse<UserDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "seller";
    public bool IsActive { get; set; } = true;
    public int CreatedById { get; set; }

    public CreateSellerCommand(CreateSellerRequest request, int createdById)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        Phone = request.Phone;
        Password = request.Password;
        Role = request.Role;
        CreatedById = createdById;
    }
}
