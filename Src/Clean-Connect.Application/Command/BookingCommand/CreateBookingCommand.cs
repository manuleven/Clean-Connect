using Clean_Connect.Application.Command.Services;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Application.Query.WorkersQuery;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Value_Objects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.BookingCommand
{
    public record CreateBookingCommand(Guid ClientId, Guid WorkerId, double Latitude, double Longitude, double RadiusInMeters, Guid ServiceTypeId, DateTime DateOfService, string TimeRange, string? CreatedBy = null) : IRequest<bool>;

    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.WorkerId)
                .NotEmpty()
                .WithMessage("Worker Id is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Invalid Id");

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("Client Id is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Invalid Id");

            RuleFor(x => x.ServiceTypeId)
                .NotEmpty()
                .WithMessage("Service type Id is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Invalid Id");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(4.0, 14.0)
                .WithMessage("Latitude must be between 4.0 and 14.0");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between 2.5 and 15.5.");


        }


    }

    public class CreateBookingHandler(IUnitOfWork repo, BookingRuleService bookingRuleService, GeocodingService geocodingService, ILogger<CreateBookingHandler> logger) : IRequestHandler<CreateBookingCommand, bool>
    {
        public async Task<bool> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {


            if (!Enum.TryParse<TimeRange>(request.TimeRange, true, out var timeRange))
            {
                throw new ValidationException("Invalid Time range value");
            }


            var checkServiceType = await repo.ServiceTypes.GetByIdAsync(request.ServiceTypeId, cancellationToken);
            if (checkServiceType == null)
            {
                logger.LogWarning("Worker creation failed. ServiceTypeId not found: {ServiceTypeId}", request.ServiceTypeId);
                throw new ValidationException("Service Type not found");
            }


            var paymentStatus = PaymentStatus.AwaitingWorkerAcceptance;

            var bookingStatus = BookingStatus.Pending;

            var location = Location.Create(request.Latitude, request.Longitude);

            var amount = checkServiceType.Amount;

            var discoverAddress = await geocodingService.GetAddressAsync(request.Latitude, request.Longitude);

            if (!discoverAddress.Contains("Nigeria"))
                throw new Exception("Location must be in Nigeria.");

            var address = Address.Create(discoverAddress);


            await bookingRuleService
                .ValidateBookingAsync(request.WorkerId,
                request.ClientId,
                request.ServiceTypeId,
                amount,
                request.DateOfService,
                timeRange,
                cancellationToken);





            var newBooking = Booking.Create(
                request.ClientId,
                request.WorkerId,
                location,
                request.DateOfService,
                DateTime.UtcNow,
                amount,
                timeRange,
                address,
                bookingStatus,
                paymentStatus,
                request.ServiceTypeId,
                request.CreatedBy
                );

            await repo.Bookings.CreateBooking(newBooking, cancellationToken);

            await repo.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}