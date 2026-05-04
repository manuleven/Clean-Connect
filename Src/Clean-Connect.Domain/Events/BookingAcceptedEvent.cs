using MediatR;


namespace Clean_Connect.Domain.Events
{
    public sealed class BookingAcceptedEvent : INotification
    {
        public Guid BookingId { get; }
        
        public BookingAcceptedEvent(Guid bookingId)
        {
            BookingId = bookingId;
           
        }
    }
}
