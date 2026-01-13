using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public sealed class WorkerCreatedDomainEvent : DomainEvent
    {
        public Guid WorkerId { get; }

        public WorkerCreatedDomainEvent(Guid workerId)
        {
            WorkerId = workerId;
        }
    }
}
