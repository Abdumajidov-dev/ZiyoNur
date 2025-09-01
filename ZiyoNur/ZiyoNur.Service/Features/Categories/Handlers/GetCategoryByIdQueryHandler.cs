using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Categories.Queries;

namespace ZiyoNur.Service.Features.Categories.Handlers;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, BaseResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Products.Category? category;

            if (request.IncludeProducts)
            {
                category = await _categoryRepository.GetCategoryWithProductsAsync(request.Id);
            }
            else if (request.IncludeSubCategories)
            {
                category = await _categoryRepository.GetCategoryWithSubCategoriesAsync(request.Id);
            }
            else
            {
                category = await _categoryRepository.GetByIdAsync(request.Id);
            }

            if (category == null)
            {
                return BaseResponse<CategoryDto>.Failure("Kategoriya topilmadi");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            // Set parent name if has parent
            if (category.ParentId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parent != null)
                {
                    categoryDto.ParentName = parent.Name;
                }
            }

            return BaseResponse<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", request.Id);
            return BaseResponse<CategoryDto>.Failure("Kategoriyani olishda xatolik yuz berdi");
        }
    }
}