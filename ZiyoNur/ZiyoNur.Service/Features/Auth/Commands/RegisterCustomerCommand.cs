using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Auth.Commands;

public class RegisterCustomerCommand : BaseRequest<BaseResponse<RegisterResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? FcmToken { get; set; }

    public RegisterCustomerCommand(RegisterCustomerRequest request)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        Phone = request.Phone;
        Email = request.Email;
        Password = request.Password;
        Address = request.Address;
        FcmToken = request.FcmToken;
    }
}
