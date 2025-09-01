using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Support;

public class SupportChat : BaseAuditableEntity, IHasDomainEvent
{
    public int CustomerId { get; set; }
    public int? AdminId { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public MessageType MessageType { get; set; } = MessageType.Text;

    [MaxLength(20)]
    public string SenderType { get; set; } = string.Empty; // customer, admin

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // File attachments
    [MaxLength(500)]
    public string? FileUrl { get; set; }
    [MaxLength(100)]
    public string? FileName { get; set; }
    [MaxLength(50)]
    public string? FileType { get; set; }

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Admin? Admin { get; set; }

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool IsSentByCustomer => SenderType == "customer";
    public bool IsSentByAdmin => SenderType == "admin";
    public bool HasAttachment => !string.IsNullOrEmpty(FileUrl);

    public void MarkAsRead(int readerId, string readerType)
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;

            DomainEvents.Add(new Events.Support.ChatMessageReadEvent(
                Id, CustomerId, AdminId, readerId, readerType));
        }
    }

    public static SupportChat CreateCustomerMessage(int customerId, string message,
        MessageType messageType = MessageType.Text, string? fileUrl = null, string? fileName = null)
    {
        return new SupportChat
        {
            CustomerId = customerId,
            Message = message,
            MessageType = messageType,
            SenderType = "customer",
            FileUrl = fileUrl,
            FileName = fileName
        };
    }

    public static SupportChat CreateAdminReply(int customerId, int adminId, string message,
        MessageType messageType = MessageType.Text, string? fileUrl = null, string? fileName = null)
    {
        return new SupportChat
        {
            CustomerId = customerId,
            AdminId = adminId,
            Message = message,
            MessageType = messageType,
            SenderType = "admin",
            FileUrl = fileUrl,
            FileName = fileName
        };
    }
}
