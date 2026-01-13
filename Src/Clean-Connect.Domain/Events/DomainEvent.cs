using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
  

  
        public abstract class DomainEvent
        {
            protected DomainEvent()
            {
                Id = Guid.NewGuid();
                OccuredOn = DateTime.UtcNow;
                EventType = GetType().Name;
            }

            public Guid Id { get; private set; }

            public DateTime OccuredOn { get; private set; }

            public string EventType { get; private set; }
        }
}

