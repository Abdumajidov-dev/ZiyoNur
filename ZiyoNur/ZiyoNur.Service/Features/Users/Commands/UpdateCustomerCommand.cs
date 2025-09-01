using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Commands;

public class UpdateCustomerCommand : BaseRequest<BaseResponse<UserDto>>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int UpdatedById { get; set; }

    public UpdateCustomerCommand(int id, UpdateCustomerRequest request, int updatedById)
    {
        Id = id;
        FirstName = request.FirstName;
        LastName = request.LastName;
        Phone = request.Phone;
        Email = request.Email;
        Address = request.Address;
        IsActive = request.IsActive;
        UpdatedById = updatedById;
    }
}
