using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.Features.Products.Commands;

namespace ZiyoNur.Service.Features.Products.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, BaseResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return BaseResponse.Failure("Mahsulot topilmadi");
            }

            // Soft delete
            await _productRepository.SoftDeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product deleted successfully: {ProductId} by admin {AdminId}", request.Id, request.DeletedById);
            return BaseResponse.Success("Mahsulot o'chirildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", request.Id);
            return BaseResponse.Failure("Mahsulotni o'chirishda xatolik yuz berdi");
        }
    }
}
