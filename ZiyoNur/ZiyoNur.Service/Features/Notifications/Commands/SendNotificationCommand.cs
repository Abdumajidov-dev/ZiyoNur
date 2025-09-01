using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Features.Notifications.Commands;

public class SendNotificationCommand : BaseRequest<BaseResponse>
{
    public int UserId { get; set; }
    public string UserType { get; set; } = string.Empty; // customer, seller, admin
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, string> ExtraData { get; set; } = new();
    public bool SendPush { get; set; } = true;

    public SendNotificationCommand(int userId, string userType, string title, string message, string notificationType)
    {
        UserId = userId;
        UserType = userType;
        Title = title;
        Message = message;
        NotificationType = notificationType;
    }
}