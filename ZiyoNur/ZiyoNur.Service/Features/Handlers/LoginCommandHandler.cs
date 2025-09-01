using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Features.Auth.Commands;
using MediatR;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Domain.Services;
using ZiyoNur.Service.Services.Interfaces;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Service.Features.Handlers;
public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        ICustomerRepository customerRepository,
        ISellerRepository sellerRepository,
        IAdminRepository adminRepository,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _sellerRepository = sellerRepository;
        _adminRepository = adminRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user based on user type
            UserDto? user = null;
            string? passwordHash = null;
            int userId = 0;

            switch (request.UserType.ToLower())
            {
                case "customer":
                    var customer = await _customerRepository.GetByPhoneAsync(request.Phone);
                    if (customer != null && customer.IsActive)
                    {
                        user = new UserDto
                        {
                            Id = customer.Id,
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Phone = customer.Phone,
                            Email = customer.Email,
                            Role = "customer",
                            UserType = "customer",
                            TotalCashback = customer.TotalCashback
                        };
                        passwordHash = customer.PasswordHash;
                        userId = customer.Id;
                    }
                    break;

                case "seller":
                    var seller = await _sellerRepository.GetByPhoneAsync(request.Phone);
                    if (seller != null && seller.IsActive)
                    {
                        user = new UserDto
                        {
                            Id = seller.Id,
                            FirstName = seller.FirstName,
                            LastName = seller.LastName,
                            Phone = seller.Phone,
                            Role = seller.Role,
                            UserType = "seller"
                        };
                        passwordHash = seller.PasswordHash;
                        userId = seller.Id;
                    }
                    break;

                case "admin":
                    var admin = await _adminRepository.GetByUsernameAsync(request.Phone) ??
                               await _adminRepository.GetByPhoneAsync(request.Phone);
                    if (admin != null && admin.IsActive)
                    {
                        user = new UserDto
                        {
                            Id = admin.Id,
                            FirstName = admin.FirstName,
                            LastName = admin.LastName,
                            Phone = admin.Phone ?? string.Empty,
                            Email = admin.Email,
                            Role = admin.Role,
                            UserType = "admin"
                        };
                        passwordHash = admin.PasswordHash;
                        userId = admin.Id;
                    }
                    break;
            }

            if (user == null || string.IsNullOrEmpty(passwordHash))
            {
                _logger.LogWarning("Login attempt with invalid phone: {Phone} for {UserType}", request.Phone, request.UserType);
                return BaseResponse<LoginResponse>.Failure("Telefon raqam yoki parol noto'g'ri");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, passwordHash))
            {
                _logger.LogWarning("Login attempt with invalid password for user: {UserId}", userId);
                return BaseResponse<LoginResponse>.Failure("Telefon raqam yoki parol noto'g'ri");
            }

            // Update FCM token if provided
            if (!string.IsNullOrEmpty(request.FcmToken))
            {
                await UpdateFcmToken(request.UserType, userId, request.FcmToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Generate JWT tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id, request.UserType);

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // Access token expires in 24 hours
                User = user
            };

            _logger.LogInformation("User {UserId} ({UserType}) logged in successfully", userId, request.UserType);
            return BaseResponse<LoginResponse>.Success(response, "Tizimga muvaffaqiyatli kirdingiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for phone: {Phone}", request.Phone);
            return BaseResponse<LoginResponse>.Failure("Tizimga kirishda xatolik yuz berdi");
        }
    }

    private async Task UpdateFcmToken(string userType, int userId, string fcmToken)
    {
        switch (userType.ToLower())
        {
            case "customer":
                var customer = await _customerRepository.GetByIdAsync(userId);
                if (customer != null)
                {
                    customer.FcmToken = fcmToken;
                    customer.LastLoginDate = DateTime.UtcNow;
                    _customerRepository.Update(customer);
                }
                break;

            case "seller":
                var seller = await _sellerRepository.GetByIdAsync(userId);
                if (seller != null)
                {
                    seller.FcmToken = fcmToken;
                    seller.LastLoginDate = DateTime.UtcNow;
                    _sellerRepository.Update(seller);
                }
                break;

            case "admin":
                var admin = await _adminRepository.GetByIdAsync(userId);
                if (admin != null)
                {
                    admin.FcmToken = fcmToken;
                    admin.LastLoginDate = DateTime.UtcNow;
                    _adminRepository.Update(admin);
                }
                break;
        }
    }
}