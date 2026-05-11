using Clean_Connect.Application.Command.BookingCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> logger;
        private readonly IMediator mediator;
        public BookingController(ILogger<BookingController> _logger, IMediator _mediator)
        {
            logger = _logger;
            mediator = _mediator;
        }


        [HttpPost("create-Booking")]

        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand booking, CancellationToken cancellationToken)
        {
            logger.LogInformation("Booking creation started");
            var result = await mediator.Send(booking, cancellationToken);
            logger.LogInformation("Booking creation completed");
            return StatusCode(201);
        }

        [HttpPost("{bookingId}/mark-as-completed-by-Worker")]
        public async Task<IActionResult> MarkJobAsCompleted(Guid bookingId, [FromBody] JobCompletedByWorkerCommand command, CancellationToken cancellationToken)
        {
            command = command with { BookingId = bookingId };

            logger.LogInformation("Awaiting client to verify booking {BookingId}", command.WorkerId, bookingId);

            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        [HttpPost("{bookingId}/Job-In-Progress")]
        public async Task<IActionResult> JobInProgress(Guid bookingId, [FromBody] JobInProgressCommand command, CancellationToken cancellationToken)
        {
            command = command with { BookingId = bookingId };

            logger.LogInformation("Awaiting client to verify booking {BookingId}", command.WorkerId, bookingId);

            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}