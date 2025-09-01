using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Products.Queries;

namespace ZiyoNur.Service.Features.Products.Handlers;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, BaseResponse<PagedResponse<ProductListDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<PagedResponse<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (products, totalCount) = await _productRepository.GetPagedProductsAsync(
                searchTerm: request.SearchTerm,
                categoryId: request.CategoryId,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice,
                inStock: request.InStock,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize);

            var productDtos = _mapper.Map<List<ProductListDto>>(products);

            // TODO: Set IsLikedByUser based on CurrentUserId
            // This requires a separate query or include in the main query

            var pagedResponse = new PagedResponse<ProductListDto>(
                productDtos,
                request.PageIndex,
                request.PageSize,
                totalCount);

            return BaseResponse<PagedResponse<ProductListDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products with filters: {SearchTerm}, {CategoryId}",
                request.SearchTerm, request.CategoryId);
            return BaseResponse<PagedResponse<ProductListDto>>.Failure("Mahsulotlarni olishda xatolik yuz berdi");
        }
    }
}