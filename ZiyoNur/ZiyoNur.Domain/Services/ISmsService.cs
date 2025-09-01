namespace ZiyoNur.Domain.Services;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message);
    Task SendOtpAsync(string phoneNumber, string code);
}
