using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Domain.Services;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Features.Users.Commands;

namespace ZiyoNur.Service.Features.Users.Handlers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, BaseResponse<UserDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<UserDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if phone already exists
            if (await _customerRepository.IsPhoneExistsAsync(request.Phone))
            {
                return BaseResponse<UserDto>.Failure("Bu telefon raqam allaqachon ro'yxatdan o'tgan");
            }

            // Check if email already exists
            if (!string.IsNullOrEmpty(request.Email) && await _customerRepository.IsEmailExistsAsync(request.Email))
            {
                return BaseResponse<UserDto>.Failure("Bu email allaqachon ro'yxatdan o'tgan");
            }

            // Create new customer
            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Address = request.Address,
                IsActive = request.IsActive,
                CreatedById = request.CreatedById,
                CreatedByType = "admin"
            };

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var customerDto = _mapper.Map<UserDto>(customer);

            _logger.LogInformation("Customer created successfully: {CustomerId} by admin {AdminId}", customer.Id, request.CreatedById);
            return BaseResponse<UserDto>.Success(customerDto, "Mijoz muvaffaqiyatli yaratildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return BaseResponse<UserDto>.Failure("Mijoz yaratishda xatolik yuz berdi");
        }
    }
}