using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Categories.Queries;

public class GetCategoriesQuery : BaseRequest<BaseResponse<List<CategoryDto>>>
{
    public bool? IsActive { get; set; }
    public bool IncludeSubCategories { get; set; } = true;
    public bool OnlyRootCategories { get; set; } = false;
    public int? ParentId { get; set; }

    public GetCategoriesQuery(bool? isActive = null, bool includeSubCategories = true, bool onlyRootCategories = false, int? parentId = null)
    {
        IsActive = isActive;
        IncludeSubCategories = includeSubCategories;
        OnlyRootCategories = onlyRootCategories;
        ParentId = parentId;
    }
}