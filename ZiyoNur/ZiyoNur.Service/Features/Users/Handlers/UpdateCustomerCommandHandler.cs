using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Features.Users.Commands;

namespace ZiyoNur.Service.Features.Users.Handlers;
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, BaseResponse<UserDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<UserDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing customer
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                return BaseResponse<UserDto>.Failure("Mijoz topilmadi");
            }

            // Check if phone already exists (exclude current customer)
            if (await _customerRepository.IsPhoneExistsAsync(request.Phone, request.Id))
            {
                return BaseResponse<UserDto>.Failure("Bu telefon raqam boshqa mijoz tomonidan ishlatilmoqda");
            }

            // Check if email already exists (exclude current customer)
            if (!string.IsNullOrEmpty(request.Email) && await _customerRepository.IsEmailExistsAsync(request.Email, request.Id))
            {
                return BaseResponse<UserDto>.Failure("Bu email boshqa mijoz tomonidan ishlatilmoqda");
            }

            // Update customer
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Phone = request.Phone;
            customer.Email = request.Email;
            customer.Address = request.Address;
            customer.IsActive = request.IsActive;
            customer.UpdatedById = request.UpdatedById;
            customer.UpdatedByType = "admin";

            _customerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var customerDto = _mapper.Map<UserDto>(customer);

            _logger.LogInformation("Customer updated successfully: {CustomerId} by admin {AdminId}", customer.Id, request.UpdatedById);
            return BaseResponse<UserDto>.Success(customerDto, "Mijoz ma'lumotlari yangilandi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", request.Id);
            return BaseResponse<UserDto>.Failure("Mijoz ma'lumotlarini yangilashda xatolik yuz berdi");
        }
    }
}