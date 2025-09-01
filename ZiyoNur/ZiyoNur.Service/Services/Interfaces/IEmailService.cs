namespace ZiyoNur.Service.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? fromName = null);
    Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody);
    Task<bool> SendTemplateEmailAsync(string to, string templateName, object model);
    Task<bool> SendOrderConfirmationAsync(string email, object orderData);
    Task<bool> SendPasswordResetAsync(string email, string resetToken);
}