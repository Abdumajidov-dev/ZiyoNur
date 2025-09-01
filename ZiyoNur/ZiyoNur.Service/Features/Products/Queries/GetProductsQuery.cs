using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Products.Queries;

public class GetProductsQuery : BaseRequest<BaseResponse<PagedResponse<ProductListDto>>>
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; } = "name";
    public bool SortDescending { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    public int? CurrentUserId { get; set; } // For liked status

    public GetProductsQuery(ProductSearchRequest request, int? currentUserId = null)
    {
        SearchTerm = request.SearchTerm;
        CategoryId = request.CategoryId;
        MinPrice = request.MinPrice;
        MaxPrice = request.MaxPrice;
        InStock = request.InStock;
        SortBy = request.SortBy ?? "name";
        SortDescending = request.SortDescending;
        PageIndex = request.PageIndex;
        PageSize = Math.Min(request.PageSize, 100); // Max 100 items per page
        CurrentUserId = currentUserId;
    }
}