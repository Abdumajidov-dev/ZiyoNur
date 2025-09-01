using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZiyoNur.Domain.Services;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.Services.Implementations;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly ICacheService _cacheService;

    public JwtTokenService(
        IConfiguration configuration,
        ILogger<JwtTokenService> logger,
        ICacheService cacheService)
    {
        _configuration = configuration;
        _logger = logger;
        _cacheService = cacheService;
    }

    public string GenerateAccessToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.MobilePhone, user.Phone),
                new(ClaimTypes.Role, user.Role),
                new("UserType", user.UserType),
            };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24), // 24 hours
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(int userId, string userType)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var cacheKey = $"refresh_token:{refreshToken}";
        var cacheValue = $"{userId}:{userType}";

        // Store refresh token in cache for 30 days
        _cacheService.SetAsync(cacheKey, cacheValue, TimeSpan.FromDays(30));

        return refreshToken;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public async Task<UserDto?> GetUserFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            if (jsonToken.ValidTo < DateTime.UtcNow)
            {
                return null; // Token expired
            }

            var userId = int.Parse(jsonToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userType = jsonToken.Claims.First(c => c.Type == "UserType").Value;
            var role = jsonToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            return new UserDto
            {
                Id = userId,
                FirstName = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "",
                LastName = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? "",
                Phone = jsonToken.Claims.First(c => c.Type == ClaimTypes.MobilePhone).Value,
                Email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Role = role,
                UserType = userType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user from token");
            return null;
        }
    }

    public async Task<string> RefreshAccessTokenAsync(string refreshToken)
    {
        var cacheKey = $"refresh_token:{refreshToken}";
        var cacheValue = await _cacheService.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cacheValue))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var parts = cacheValue.Split(':');
        var userId = int.Parse(parts[0]);
        var userType = parts[1];

        // TODO: Get user details from database and generate new access token
        // This is a simplified version
        throw new NotImplementedException("RefreshAccessTokenAsync needs user repository implementation");
    }
}
