using System.Collections.Generic;
namespace ZiyoNur.Domain.Common;

public interface IHasDomainEvent
{
    public List<BaseEvent> DomainEvents { get; set; }
}
