using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Products.Queries;

namespace ZiyoNur.Service.Features.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, BaseResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetProductWithCategoryAsync(request.Id);
            if (product == null)
            {
                return BaseResponse<ProductDto>.Failure("Mahsulot topilmadi");
            }

            var productDto = _mapper.Map<ProductDto>(product);

            // Increment view count asynchronously (don't block response)
            _ = Task.Run(async () =>
            {
                try
                {
                    // Get fresh product instance for update
                    var productForUpdate = await _productRepository.GetByIdAsync(request.Id);
                    if (productForUpdate != null)
                    {
                        productForUpdate.IncrementViewCount();
                        _productRepository.Update(productForUpdate);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating view count for product {ProductId}", request.Id);
                }
            });

            // TODO: Set IsLikedByUser and IsInUserCart based on CurrentUserId
            if (request.CurrentUserId.HasValue)
            {
                // This would require additional repository calls
                // productDto.IsLikedByUser = await CheckIfProductIsLiked(request.Id, request.CurrentUserId.Value);
                // productDto.IsInUserCart = await CheckIfProductInCart(request.Id, request.CurrentUserId.Value);
            }

            return BaseResponse<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", request.Id);
            return BaseResponse<ProductDto>.Failure("Mahsulotni olishda xatolik yuz berdi");
        }
    }
}