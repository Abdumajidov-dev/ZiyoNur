using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Categories.Commands;

namespace ZiyoNur.Service.Features.Categories.Handlers;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, BaseResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing category
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                return BaseResponse<CategoryDto>.Failure("Kategoriya topilmadi");
            }

            // Validate parent category if provided
            if (request.ParentId.HasValue)
            {
                if (request.ParentId.Value == request.Id)
                {
                    return BaseResponse<CategoryDto>.Failure("Kategoriya o'zini ota kategoriya qilib bo'lmaydi");
                }

                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value);
                if (parentCategory == null || !parentCategory.IsActive)
                {
                    return BaseResponse<CategoryDto>.Failure("Ota kategoriya topilmadi");
                }

                // Check for circular reference
                if (await WouldCreateCircularReference(request.Id, request.ParentId.Value))
                {
                    return BaseResponse<CategoryDto>.Failure("Bu o'zgarish siklik bog'lanish yaratadi");
                }
            }

            // Update category
            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentId = request.ParentId;
            category.ImageUrl = request.ImageUrl;
            category.SortOrder = request.SortOrder;
            category.IsActive = request.IsActive;
            category.UpdatedById = request.UpdatedById;
            category.UpdatedByType = "admin";

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = _mapper.Map<CategoryDto>(category);

            _logger.LogInformation("Category updated successfully: {CategoryId} by admin {AdminId}", category.Id, request.UpdatedById);
            return BaseResponse<CategoryDto>.Success(categoryDto, "Kategoriya yangilandi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", request.Id);
            return BaseResponse<CategoryDto>.Failure("Kategoriyani yangilashda xatolik yuz berdi");
        }
    }

    private async Task<bool> WouldCreateCircularReference(int categoryId, int parentId)
    {
        // Simple circular reference check - can be improved
        var visited = new HashSet<int> { categoryId };
        int? currentParentId = parentId; // Fixed: Declare as nullable int from start

        while (currentParentId.HasValue)
        {
            if (visited.Contains(currentParentId.Value))
            {
                return true; // Circular reference detected
            }

            visited.Add(currentParentId.Value);
            var parent = await _categoryRepository.GetByIdAsync(currentParentId.Value);
            currentParentId = parent?.ParentId;
        }

        return false;
    }
}