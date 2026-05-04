using Clean_Connect.Application.Command.BookingCommand;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        public async Task<ActionResult> CreateBooking([FromBody] CreateBookingCommand booking, CancellationToken cancellationToken)
        {
            logger.LogInformation("Booking creation started");
            var result = await mediator.Send(booking, cancellationToken);
            logger.LogInformation("Booking creation completed");
            return StatusCode(201);
        }
    }
}