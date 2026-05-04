using Azure.Core;
using Clean_Connect.Application.Command.Services;
using Clean_Connect.Application.Command.WorkerCommands;
using Clean_Connect.Application.Interface.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.ClientCommands
{
    public record MarkAsCompletedCommand(Guid BookingId, Guid ClientId) : IRequest<bool>;

    public class  MarkAsCompletedCommandValidator : AbstractValidator<MarkAsCompletedCommand>
    {
        public MarkAsCompletedCommandValidator()
        {
            RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("BookingId is required.");

            RuleFor(x => x.ClientId)
            .NotEmpty()
            .Must(id => id != Guid.Empty)
            .WithMessage("ClientId is required."); 
        }
        
    }

    public class MarkAsCompletedHandler : IRequestHandler<MarkAsCompletedCommand, bool>
    {
        private readonly IUnitOfWork repo;
        private readonly MarkAsCompletedService service;
        private readonly ILogger<AcceptBookingHandler> logger;

        public MarkAsCompletedHandler(IUnitOfWork _repo, MarkAsCompletedService _service, ILogger<AcceptBookingHandler> _logger)
        {
            repo = _repo;
            service = _service;
            logger = _logger;
        }

        public async Task <bool> Handle(MarkAsCompletedCommand request, CancellationToken cancellationToken)
        {
            var booking = await repo.Bookings.GetBookingById(request.BookingId, cancellationToken)
               ?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

            await service.MarkAsCompletedServiceAsync(request.BookingId, request.ClientId, cancellationToken);

            booking.MarkAsCompleted();

            logger.LogInformation("Worker {WorkerId} Completed booking {BookingId}", booking.WorkerId, request.BookingId);

            await repo.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Worker {WorkerId} Completed booking {BookingId}", booking.WorkerId, request.BookingId);
            return true;

            
        }
    }
  
}
