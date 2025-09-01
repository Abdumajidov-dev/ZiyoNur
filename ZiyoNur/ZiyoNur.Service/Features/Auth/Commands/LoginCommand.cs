using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Auth.Commands;

public class LoginCommand : BaseRequest<BaseResponse<LoginResponse>>
{
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FcmToken { get; set; }
    public string UserType { get; set; } = "customer"; // customer, seller, admin

    public LoginCommand(LoginRequest request, string userType = "customer")
    {
        Phone = request.Phone;
        Password = request.Password;
        FcmToken = request.FcmToken;
        UserType = userType;
    }
}
