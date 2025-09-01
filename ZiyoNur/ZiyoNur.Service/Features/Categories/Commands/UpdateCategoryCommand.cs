using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Categories.Commands;

public class UpdateCategoryCommand : BaseRequest<BaseResponse<CategoryDto>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int UpdatedById { get; set; }

    public UpdateCategoryCommand(int id, UpdateCategoryRequest request, int updatedById)
    {
        Id = id;
        Name = request.Name;
        Description = request.Description;
        ParentId = request?.ParentId;
        ImageUrl = request?.ImageUrl;
        SortOrder = request.SortOrder;
        IsActive = request.IsActive;
        UpdatedById = updatedById;
    }
}
