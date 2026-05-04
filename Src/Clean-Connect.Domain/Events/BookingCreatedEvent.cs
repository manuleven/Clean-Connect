using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public sealed class BookingCreatedEvent : DomainEvent
    {
         public Guid BookingId { get;}

        public BookingCreatedEvent(Guid bookingId)
        {
            BookingId = bookingId;
        }
    }
}
