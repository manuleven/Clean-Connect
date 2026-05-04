using Clean_Connect.Application.Command.WorkerCommands;
using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Query.WorkersQuery;
using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<WorkerController> _logger;
        public WorkerController(IMediator mediator, ILogger<WorkerController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [HttpPost("register-worker")]

        public async Task<ActionResult> RegisterWorker([FromBody] CreateWorkerCommand worker, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registeration operation started");

            var result = await _mediator.Send(worker, cancellationToken);
            _logger.LogInformation("Registeration operation completed");
            return Ok(result);
        }

        [HttpPost("{bookingId}/accept")]
        public async Task<IActionResult> AcceptBooking(Guid bookingId, [FromBody] AcceptBookingCommand command, CancellationToken cancellationToken)
        {
            command = command with { BookingId = bookingId };

            _logger.LogInformation("Worker {WorkerId} accepting booking {BookingId}",
                command.WorkerId, bookingId);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        [HttpPost("{bookingId}/Reject")]
        public async Task<IActionResult> RejectBooking(Guid bookingId, [FromBody] RejectBookingCommand command, CancellationToken cancellationToken)
        {
            command = command with { BookingId = bookingId };

            _logger.LogInformation("Worker {WorkerId} Rejecting booking {BookingId}",
                command.WorkerId, bookingId);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut("update-worker")]

        public async Task<ActionResult> UpdateWorker([FromBody] UpdateWorkerCommand worker, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update operation started");
            var result = await _mediator.Send(worker, cancellationToken);
            _logger.LogInformation("Update operation completed");
            return Ok(result);
        }

        [HttpGet("all-worker")]

        public async Task<ActionResult> GetAllWorkers(CancellationToken cancellationToken)
        {
            //_logger.LogInformation($"GetAllWorkers: {query}");

            var allWorker = await _mediator.Send(new GetAllWorkersQuery(), cancellationToken);
            _logger.LogInformation("All worker fetched succesfully");
            return Ok(allWorker);
        }

        [HttpGet("get-worker-by-id")]
        public async Task<ActionResult> GetWorkersById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting worker by id");

            var worker = await _mediator.Send(new GetWorkerByIdQuery(id), cancellationToken);

            _logger.LogInformation("Worker fetched succesfully");
            return Ok(worker);
        }
        [HttpGet("get-worker-booking-by-id")]
        public async Task<ActionResult> GetWorkerBookingsById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting worker booking by id");

            var worker = await _mediator.Send(new GetAllWorkerBookingsQuery(id), cancellationToken);

            _logger.LogInformation("Worker booking fetched succesfully");
            return Ok(worker);
        }

        [HttpPost("nearby")]
        public async Task<ActionResult<List<WorkerDto>>> GetNearbyWorkers([FromBody] GetNearByWorkersQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
