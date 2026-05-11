using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Services
{
    public class MarkAsCompletedService
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<MarkAsCompletedService> logger;

        public MarkAsCompletedService(IUnitOfWork _repo, ILogger<MarkAsCompletedService> _logger)
        {
            repo = _repo;
            logger = _logger;
        }

        public async Task MarkAsCompletedServiceAsync(Guid bookingId, Guid clientId, CancellationToken cancellationToken)
        {
            var booking = await repo.Bookings.GetBookingById(bookingId, cancellationToken);
            if (booking == null)
            {
                logger.LogWarning("Booking operation failed. Booking not found: {BookingId}", bookingId);
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
            }
            if (booking.ClientId != clientId)
            {
                logger.LogWarning("Booking operation failed. Client {ClientId} is not assigned to booking {BookingId}", clientId, bookingId);
                throw new UnauthorizedAccessException($"Worker with ID {clientId} is not assigned to booking {bookingId}.");
            }
            if (booking.BookingStatus != BookingStatus.InProgress && booking.BookingStatus != BookingStatus.MarkAsPaid)
            {
                logger.LogWarning("Booking operation failed. Booking {BookingId} is not in Progress ", bookingId);
                throw new InvalidOperationException($"Booking with ID {bookingId} must be paid or in progress before it can be marked as completed.");
            }
            if (booking.PaymentStatus != PaymentStatus.Successful)
            {
                logger.LogWarning("Booking operation failed. Booking {BookingId} has not been paid", bookingId);
                throw new InvalidOperationException($"Booking with ID {bookingId} must be paid before it can be marked as completed.");
            }
        }
    }
}
