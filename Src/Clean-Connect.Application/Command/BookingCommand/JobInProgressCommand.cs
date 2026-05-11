using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.BookingCommand
{
    public record JobInProgressCommand(Guid WorkerId, Guid BookingId) : IRequest<bool>;

    public class JobInProgressValidator : AbstractValidator<JobInProgressCommand>
    {
        public JobInProgressValidator()
        {
            RuleFor(x => x.WorkerId)
                .NotEmpty()
                .WithMessage("Worker Id is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Invalid Id");

            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking Id is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Invalid Id");
        }
    }
    public class JobInProgressHandler : IRequestHandler<JobInProgressCommand, bool>
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<JobInProgressHandler> logger;

        public JobInProgressHandler(IUnitOfWork _repo, ILogger<JobInProgressHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }
        public async Task<bool> Handle(JobInProgressCommand request, CancellationToken cancellationToken)
        {
            var booking = await repo.Bookings.GetBookingById(request.BookingId, cancellationToken);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");
            }
            if (booking.WorkerId != request.WorkerId)
            {
                throw new UnauthorizedAccessException($"Worker with ID {request.WorkerId} is not assigned to booking {request.BookingId}.");
            }
            if (booking.BookingStatus != BookingStatus.MarkAsPaid)
            {
                throw new InvalidOperationException($"Booking with ID {request.BookingId} is not in paid status and cannot be marked as in progress.");
            }

            if(booking.DateOfBooking != DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException($"Booking with ID {request.BookingId} is not scheduled for today and cannot be marked as in progress.");
            } 

            booking.StartJob();
            await repo.Bookings.UpdateBooking(booking, cancellationToken);
            return true;
        }

    }
}