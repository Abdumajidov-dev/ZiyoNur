namespace ZiyoNur.Service.Configuration;

public class ServiceConfiguration
{
    public JwtConfiguration Jwt { get; set; } = new();
    public FileStorageConfiguration FileStorage { get; set; } = new();
    public ExternalServicesConfiguration ExternalServices { get; set; } = new();
    public NotificationConfiguration Notifications { get; set; } = new();
}

public class JwtConfiguration
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 1440; // 24 hours
    public int RefreshTokenExpiryDays { get; set; } = 30;
}

public class FileStorageConfiguration
{
    public string UploadPath { get; set; } = "wwwroot/uploads";
    public string BaseUrl { get; set; } = "https://localhost:7000/uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedImageTypes { get; set; } = { ".jpg", ".jpeg", ".png", ".gif" };
    public string[] AllowedDocumentTypes { get; set; } = { ".pdf", ".doc", ".docx", ".txt" };
}

public class ExternalServicesConfiguration
{
    public PaymeConfiguration Payme { get; set; } = new();
    public UzcardConfiguration Uzcard { get; set; } = new();
    public ClickConfiguration Click { get; set; } = new();
    public SmsConfiguration Sms { get; set; } = new();
    public EmailConfiguration Email { get; set; } = new();
    public FcmConfiguration Fcm { get; set; } = new();
}

public class PaymeConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool IsTestMode { get; set; } = true;
}

public class UzcardConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool IsTestMode { get; set; } = true;
}

public class ClickConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool IsTestMode { get; set; } = true;
}

public class SmsConfiguration
{
    public string Provider { get; set; } = "eskiz"; // eskiz, sms.uz
    public string ApiUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = "Kutubxona";
    public bool IsEnabled { get; set; } = true;
}

public class EmailConfiguration
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Kutubxona";
    public bool EnableSsl { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
}

public class FcmConfiguration
{
    public string ServerKey { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public class NotificationConfiguration
{
    public bool EnablePushNotifications { get; set; } = true;
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnableSmsNotifications { get; set; } = false;
    public int BatchSize { get; set; } = 100;
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
}