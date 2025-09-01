using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Features.Categories.Commands;

public class DeleteCategoryCommand : BaseRequest<BaseResponse>
{
    public int Id { get; set; }
    public int DeletedById { get; set; }
    public bool ForceDelete { get; set; } = false; // If true, delete even if has products/subcategories

    public DeleteCategoryCommand(int id, int deletedById, bool forceDelete = false)
    {
        Id = id;
        DeletedById = deletedById;
        ForceDelete = forceDelete;
    }
}
