using ZiyoNur.Service.DTOs.Auth;

namespace ZiyoNur.Service.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(UserDto user);
    string GenerateRefreshToken(int userId, string userType);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetUserFromTokenAsync(string token);
    Task<string> RefreshAccessTokenAsync(string refreshToken);
}