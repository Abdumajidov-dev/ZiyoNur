using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Commands;

public class CreateCustomerCommand : BaseRequest<BaseResponse<UserDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public int CreatedById { get; set; }

    public CreateCustomerCommand(RegisterCustomerRequest request, int createdById)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        Phone = request.Phone;
        Email = request.Email;
        Password = request.Password;
        Address = request.Address;
        CreatedById = createdById;
    }
}