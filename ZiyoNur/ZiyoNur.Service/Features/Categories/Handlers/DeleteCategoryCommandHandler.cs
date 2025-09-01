using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.Features.Categories.Commands;

namespace ZiyoNur.Service.Features.Categories.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, BaseResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing category
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                return BaseResponse.Failure("Kategoriya topilmadi");
            }

            // Check if category has products
            if (!request.ForceDelete && await _categoryRepository.HasProductsAsync(request.Id))
            {
                return BaseResponse.Failure("Bu kategoriyada mahsulotlar mavjud. Avval mahsulotlarni boshqa kategoriyaga o'tkazing");
            }

            // Check if category has subcategories
            if (!request.ForceDelete && await _categoryRepository.HasSubCategoriesAsync(request.Id))
            {
                return BaseResponse.Failure("Bu kategoriyada ichki kategoriyalar mavjud. Avval ularni o'chiring yoki boshqa kategoriyaga o'tkazing");
            }

            // Soft delete
            await _categoryRepository.SoftDeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category deleted successfully: {CategoryId} by admin {AdminId}", request.Id, request.DeletedById);
            return BaseResponse.Success("Kategoriya o'chirildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", request.Id);
            return BaseResponse.Failure("Kategoriyani o'chirishda xatolik yuz berdi");
        }
    }
}