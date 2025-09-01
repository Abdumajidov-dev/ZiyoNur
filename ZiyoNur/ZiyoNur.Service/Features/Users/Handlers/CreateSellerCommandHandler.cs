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

public class CreateSellerCommandHandler : IRequestHandler<CreateSellerCommand, BaseResponse<UserDto>>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSellerCommandHandler> _logger;

    public CreateSellerCommandHandler(
        ISellerRepository sellerRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateSellerCommandHandler> logger)
    {
        _sellerRepository = sellerRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<UserDto>> Handle(CreateSellerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if phone already exists
            if (await _sellerRepository.IsPhoneExistsAsync(request.Phone))
            {
                return BaseResponse<UserDto>.Failure("Bu telefon raqam allaqachon ro'yxatdan o'tgan");
            }

            // Create new seller
            var seller = new Seller
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = request.Role,
                IsActive = request.IsActive,
                CreatedById = request.CreatedById,
                CreatedByType = "admin"
            };

            await _sellerRepository.AddAsync(seller);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var sellerDto = _mapper.Map<UserDto>(seller);

            _logger.LogInformation("Seller created successfully: {SellerId} by admin {AdminId}", seller.Id, request.CreatedById);
            return BaseResponse<UserDto>.Success(sellerDto, "Sotuvchi muvaffaqiyatli yaratildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seller");
            return BaseResponse<UserDto>.Failure("Sotuvchi yaratishda xatolik yuz berdi");
        }
    }
}