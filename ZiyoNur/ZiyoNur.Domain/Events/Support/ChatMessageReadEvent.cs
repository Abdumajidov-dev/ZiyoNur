using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Support;

public class ChatMessageReadEvent : BaseEvent
{
    public int MessageId { get; }
    public int CustomerId { get; }
    public int? AdminId { get; }
    public int ReaderId { get; }
    public string ReaderType { get; }

    public ChatMessageReadEvent(int messageId, int customerId, int? adminId,
        int readerId, string readerType)
    {
        MessageId = messageId;
        CustomerId = customerId;
        AdminId = adminId;
        ReaderId = readerId;
        ReaderType = readerType;
    }
}
