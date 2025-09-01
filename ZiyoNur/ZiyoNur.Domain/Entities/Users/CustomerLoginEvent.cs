using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Users;

public class CustomerLoginEvent : BaseEvent
{
    public int CustomerId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }

    public CustomerLoginEvent(int customerId, string ipAddress, string userAgent)
    {
        CustomerId = customerId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}
