using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Categories.Commands;

namespace ZiyoNur.Service.Features.Categories.Handlers;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, BaseResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate parent category if provided
            if (request.ParentId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value);
                if (parentCategory == null || !parentCategory.IsActive)
                {
                    return BaseResponse<CategoryDto>.Failure("Ota kategoriya topilmadi");
                }
            }

            // Create new category
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId,
                ImageUrl = request.ImageUrl,
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedById = request.CreatedById,
                CreatedByType = "admin"
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = _mapper.Map<CategoryDto>(category);

            _logger.LogInformation("Category created successfully: {CategoryId} by admin {AdminId}", category.Id, request.CreatedById);
            return BaseResponse<CategoryDto>.Success(categoryDto, "Kategoriya yaratildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return BaseResponse<CategoryDto>.Failure("Kategoriya yaratishda xatolik yuz berdi");
        }
    }
}