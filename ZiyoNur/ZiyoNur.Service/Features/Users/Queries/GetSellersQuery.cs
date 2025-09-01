using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Features.Users.Queries;
public class GetSellersQuery : BaseRequest<BaseResponse<PagedResponse<UserDto>>>
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
    public string? SortBy { get; set; } = "firstName";
    public bool SortDescending { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;
}