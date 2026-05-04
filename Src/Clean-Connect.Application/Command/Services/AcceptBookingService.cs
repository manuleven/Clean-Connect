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
    public class AcceptBookingService
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<AcceptBookingService> logger;

        public AcceptBookingService(IUnitOfWork _repo, ILogger<AcceptBookingService> _logger)
        {
            repo = _repo;
            logger = _logger;
        }

        public async Task AcceptBookingAsync(Guid bookingId, Guid workerId, CancellationToken cancellationToken)
        {
            var booking = await repo.Bookings.GetBookingById(bookingId, cancellationToken);
            if (booking == null)
            {
                logger.LogWarning("Booking operation failed. Booking not found: {BookingId}", bookingId);
               throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
            }
            if (booking.WorkerId != workerId)
            {
                logger.LogWarning("Booking operation failed. Worker {WorkerId} is not assigned to booking {BookingId}", workerId, bookingId);
                throw new UnauthorizedAccessException($"Worker with ID {workerId} is not assigned to booking {bookingId}.");
            }
            if (booking.BookingStatus != BookingStatus.Pending)
            {
                logger.LogWarning("Booking operation failed. Booking {BookingId} is not in pending status", bookingId);
                throw new InvalidOperationException($"Booking with ID {bookingId} is not in pending status and cannot be accepted.");
            }

            

        }
    }
}
