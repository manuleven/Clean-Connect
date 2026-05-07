using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Events;
using Clean_Connect.Domain.Utilities;
using Clean_Connect.Domain.Value_Objects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Entities
{
    public class Booking : BaseEntity
    {
        private Booking() { }

        private Booking(Guid clientId, Guid workerId, DateTime dateOfBooking,Location location, DateTime dateOfService, decimal amount, TimeRange timeRange, PaymentStatus paymentStatus, Address address, Guid serviceTypeId, BookingStatus bookingStatus, string? createdBy = null)
        {
            ClientId = clientId;
            WorkerId = workerId;
            ServiceTypeId = serviceTypeId;
            Amount = amount;
            PaymentStatus = paymentStatus;
            Location = location;
            Address = address;
            DateOfBooking = dateOfBooking;
            DateOfService = dateOfService;
            BookingStatus = bookingStatus;
        }

        public Client Client { get; private set; }
        private readonly List<INotification> _domainEvents = new();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();


        public Worker Worker { get; private set; }

        public PaymentStatus PaymentStatus { get; private set; }
        public Address Address { get; private set; }
        public Guid ClientId { get; private set; } = default!;

        public Guid WorkerId { get; private set; } = default!;

        public Location Location { get; private set; }

        public DateTime DateOfBooking { get; private set; } = default!;
        
        public Ratings Ratings { get; private set; }
        public TimeRange TimeRange { get; private set; } = default!;

        public decimal Amount { get; private set; } = default!;
        public DateTime DateOfService { get; private set; } = default!;
        public ServiceType ServiceType { get; private set; }
        public Guid ServiceTypeId { get; private set; } = default!;

        public BookingStatus BookingStatus { get; private set; } = default!;



        public static Booking Create(Guid clientId, Guid workerId, Location location, DateTime dateOfService, DateTime dateOfBooking, decimal amount, TimeRange timeRange, Address address, BookingStatus bookingStatus, PaymentStatus paymentStatus, Guid serviceTypeId, string? createdBy = null)
        {
            var booking = new Booking(clientId, workerId, dateOfBooking,location, dateOfService, amount, timeRange, paymentStatus, address, serviceTypeId, bookingStatus, createdBy);
            booking.AddDomainEvent(new BookingCreatedEvent(booking.Id));
            booking.UpdateMetadata(createdBy);
            return booking;

        }

        public void Accept()
        {
            if (BookingStatus != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Only pending bookings can be accepted.");
            }
            BookingStatus = BookingStatus.AcceptedAwaitingPayment;
            PaymentStatus = PaymentStatus.Pending;

            _domainEvents.Add(new BookingAcceptedEvent(Id));
        }

        public void Reject()
        {
            if (BookingStatus != BookingStatus.Pending)
            {
                throw new InvalidOperationException("Only pending bookings can be rejected.");
            }
            BookingStatus = BookingStatus.Rejected;
            PaymentStatus = PaymentStatus.Canceled;
            

        }

        public void StartJob()
        {
            if (BookingStatus != BookingStatus.AcceptedAwaitingPayment)
            {
                throw new InvalidOperationException("Only accepted bookings can be started.");
            }

            BookingStatus = BookingStatus.InProgress;
        }

        public void MarkAsPaid()
        {
            if (BookingStatus != BookingStatus.AcceptedAwaitingPayment)
            {
                throw new InvalidOperationException("Only accepted bookings can be marked as paid.");
            }
            BookingStatus = BookingStatus.MarkAsPaid;
            PaymentStatus = PaymentStatus.Successful;
        }   

        public void MarkAsCompleted()
        {
            if (BookingStatus != BookingStatus.InProgress)
            {
                throw new InvalidOperationException("Only in-progress bookings can be marked as completed.");
            }
            BookingStatus = BookingStatus.Completed;
        }
    }
}
