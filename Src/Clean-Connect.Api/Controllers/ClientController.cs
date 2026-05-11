using Clean_Connect.Application.Command.ClientCommands;
using Clean_Connect.Application.Command.WorkerCommands;
using Clean_Connect.Application.Query.ClientQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> logger;
        private readonly IMediator mediator;
        public ClientController(ILogger<ClientController> _logger, IMediator _mediator)
        {
            logger = _logger;
            mediator = _mediator;
        }

        [HttpPost("register-client")]

        public async Task<ActionResult> RegisterClient([FromBody] CreateClientCommand client, CancellationToken cancellationToken)
        {
            logger.LogInformation("Client registration started");
            var result = await mediator.Send(client, cancellationToken);
            logger.LogInformation("Client registration completed");
            return Ok(result);
        }

        [HttpGet("get-client-by-id")]

        public async Task<ActionResult> GetClientById(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching client by ID");
            var client = new GetClientByIdQuery(id);
            var result = await mediator.Send(client, cancellationToken);
            logger.LogInformation("Client fetched successfully");
            return Ok(result);
        }
        [HttpGet("get-client-bookings-by-id")]

        public async Task<ActionResult> GetClientBookingsById(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching client bookings by ID");
            var client = new GetAllClientBookingsQuery(id);
            var result = await mediator.Send(client, cancellationToken);
            logger.LogInformation("Client bookings fetched successfully");
            return Ok(result);
        }

        [HttpGet("all-clients")]
        public async Task<ActionResult> GetAllClients(CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching all clients");

            var result = await mediator.Send(new GetAllClientQuery(), cancellationToken);
            logger.LogInformation("All clients fetched successfully");
            return Ok(result);
        }

        [HttpPut("update-client")]

        public async Task<ActionResult> UpdateClient([FromBody] UpdateClientCommand client, CancellationToken cancellationToken)
        {
            logger.LogInformation("Client update started");
            var result = await mediator.Send(client, cancellationToken);
            logger.LogInformation("Client update completed");
            return Ok(result);
        }

        [HttpPost("{bookingId}/markasCompleted")]
        public async Task<IActionResult> MarkBookingAsCompleted(Guid bookingId, [FromBody] MarkAsCompletedCommand command, CancellationToken cancellationToken)
        {
            command = command with { BookingId = bookingId };

            logger.LogInformation("Worker {WorkerId} Completed booking {BookingId}",command.ClientId, bookingId);

            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("RateWorker")]

        public async Task<IActionResult> RateWorker([FromBody] CreateRatingCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker rating started");
            var result = await mediator.Send(command, cancellationToken);
            logger.LogInformation("Worker rating completed");
            return Ok(result);
        }
    }
}
