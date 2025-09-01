using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Categories.Queries;

public class GetCategoryByIdQuery : BaseRequest<BaseResponse<CategoryDto>>
{
    public int Id { get; set; }
    public bool IncludeProducts { get; set; } = false;
    public bool IncludeSubCategories { get; set; } = true;

    public GetCategoryByIdQuery(int id, bool includeProducts = false, bool includeSubCategories = true)
    {
        Id = id;
        IncludeProducts = includeProducts;
        IncludeSubCategories = includeSubCategories;
    }
}