using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Commands;

public class UpdateSellerCommand : BaseRequest<BaseResponse<UserDto>>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "seller";
    public bool IsActive { get; set; }
    public int UpdatedById { get; set; }

    public UpdateSellerCommand(int id, UpdateSellerRequest request, int updatedById)
    {
        Id = id;
        FirstName = request.FirstName;
        LastName = request.LastName;
        Phone = request.Phone;
        Role = request.Role;
        IsActive = request.IsActive;
        UpdatedById = updatedById;
    }
}