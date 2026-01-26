using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public sealed class ServiceTypeCreatedEvent : DomainEvent
    {
       public Guid ServiceId { get; }

        public ServiceTypeCreatedEvent(Guid serviceId)
        {
            ServiceId = serviceId;
        }
    }
}
