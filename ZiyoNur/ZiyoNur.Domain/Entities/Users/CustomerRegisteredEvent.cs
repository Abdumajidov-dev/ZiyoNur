using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Users;

public class CustomerRegisteredEvent : BaseEvent
{
    public int CustomerId { get; }
    public string CustomerName { get; }
    public string Phone { get; }

    public CustomerRegisteredEvent(int customerId, string customerName, string phone)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        Phone = phone;
    }
}
