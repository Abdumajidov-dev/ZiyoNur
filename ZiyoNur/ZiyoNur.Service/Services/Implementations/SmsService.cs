using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.Services.Implementations;

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            // TODO: Implement SMS sending via SMS.uz or similar service
            _logger.LogInformation("Sending SMS to {PhoneNumber}: {Message}", phoneNumber, message);

            // Simulate external API call
            await Task.Delay(100);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public async Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message)
    {
        var tasks = phoneNumbers.Select(phone => SendSmsAsync(phone, message));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string code)
    {
        var message = $"Kutubxona tasdiqlash kodi: {code}. Kodni hech kimga bermang!";
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendOrderNotificationAsync(string phoneNumber, string orderNumber)
    {
        var message = $"Buyurtmangiz #{orderNumber} tasdiqlandi. Tafsilotlar uchun ilovani oching.";
        return await SendSmsAsync(phoneNumber, message);
    }
}
