using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Orders;

namespace ZiyoNur.Service.Features.Orders.Queries;

public class GetCustomerOrdersQuery : BaseRequest<BaseResponse<PagedResponse<OrderDto>>>
{
    public int CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;

    public GetCustomerOrdersQuery(int customerId)
    {
        CustomerId = customerId;
    }
}