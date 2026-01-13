using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public sealed class ClientCreatedDomainEvent : DomainEvent
    {
        public Guid ClientId { get; }
        public ClientCreatedDomainEvent(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}
