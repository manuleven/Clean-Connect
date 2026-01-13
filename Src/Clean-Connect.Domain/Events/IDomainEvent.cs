using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccuredOn { get; }
        string EventType { get; }
    }
}
