using ZiyoNur.Service.DTOs.Common;

namespace ZiyoNur.Service.DTOs.Notifications;

public class NotificationDto : AuditableDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public Dictionary<string, object> ExtraData { get; set; } = new();
}