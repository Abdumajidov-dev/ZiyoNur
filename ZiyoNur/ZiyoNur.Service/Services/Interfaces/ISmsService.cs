namespace ZiyoNur.Service.Services.Interfaces;

public interface ISmsService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message);
    Task<bool> SendOtpAsync(string phoneNumber, string code);
    Task<bool> SendOrderNotificationAsync(string phoneNumber, string orderNumber);
}
