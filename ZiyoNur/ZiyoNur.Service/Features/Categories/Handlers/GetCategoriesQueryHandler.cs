using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Categories.Queries;

namespace ZiyoNur.Service.Features.Categories.Handlers;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, BaseResponse<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoriesQueryHandler> _logger;

    public GetCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<GetCategoriesQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.Products.Category> categories;

            if (request.OnlyRootCategories)
            {
                categories = (await _categoryRepository.GetRootCategoriesAsync()).ToList();
            }
            else if (request.ParentId.HasValue)
            {
                categories = (await _categoryRepository.GetSubCategoriesAsync(request.ParentId.Value)).ToList();
            }
            else
            {
                categories = (await _categoryRepository.GetActiveCategoriesAsync()).ToList();
            }

            // Apply active filter if specified
            if (request.IsActive.HasValue)
            {
                categories = categories.Where(c => c.IsActive == request.IsActive.Value).ToList();
            }

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

            // Load subcategories if requested
            if (request.IncludeSubCategories)
            {
                foreach (var categoryDto in categoryDtos)
                {
                    if (categoryDto.ParentId == null) // Only for root categories
                    {
                        var subCategories = await _categoryRepository.GetSubCategoriesAsync(categoryDto.Id);
                        categoryDto.SubCategories = _mapper.Map<List<CategoryDto>>(subCategories.Where(sc => request.IsActive == null || sc.IsActive == request.IsActive.Value));
                    }
                }
            }

            return BaseResponse<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return BaseResponse<List<CategoryDto>>.Failure("Kategoriyalarni olishda xatolik yuz berdi");
        }
    }
}