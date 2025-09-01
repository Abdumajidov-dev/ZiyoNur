using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Categories.Commands;

public class CreateCategoryCommand : BaseRequest<BaseResponse<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public int CreatedById { get; set; }

    public CreateCategoryCommand(CreateCategoryRequest request, int createdById)
    {
        Name = request.Name;
        Description = request.Description;
        ParentId = request.ParentId;
        ImageUrl = request.ImageUrl;
        SortOrder = request.SortOrder;
        CreatedById = createdById;
    }
}
