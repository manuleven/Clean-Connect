using Clean_Connect.Application.Command.WorkerCommands;
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

        [HttpPut("update-worker")]

        public async Task<ActionResult> UpdateWorker([FromBody] UpdateWorkerCommand worker, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update operation started");
            var result = await _mediator.Send(worker, cancellationToken);
            _logger.LogInformation("Update operation completed");
            return Ok(result);
        }

        [HttpGet("all-worker")]

        public async Task<ActionResult> GetAllWorkers(GetAllWorkersQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GetAllWorkers: {query}");

            var allWorker = await _mediator.Send(query, cancellationToken);
            _logger.LogInformation("All worker fetched succesfully");
            return Ok(allWorker);
        }

        [HttpGet("get-by-worker-id")]
        public async Task<ActionResult>GetWorkersById(GetWorkerByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting worker by id");

            var worker = await _mediator.Send(query,cancellationToken);

            _logger.LogInformation("Worker fetched succesfully");
            return Ok(worker);
        }

    }
}
