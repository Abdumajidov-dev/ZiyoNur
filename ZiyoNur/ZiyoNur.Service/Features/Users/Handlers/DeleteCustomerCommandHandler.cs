using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.Features.Users.Commands;

namespace ZiyoNur.Service.Features.Users.Handlers;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, BaseResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing customer
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                return BaseResponse.Failure("Mijoz topilmadi");
            }

            // Soft delete
            await _customerRepository.SoftDeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer deleted successfully: {CustomerId} by admin {AdminId}", request.Id, request.DeletedById);
            return BaseResponse.Success("Mijoz o'chirildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", request.Id);
            return BaseResponse.Failure("Mijozni o'chirishda xatolik yuz berdi");
        }
    }
}