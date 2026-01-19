using Clean_Connect.Application.Command.ClientCommands;
using Clean_Connect.Application.Query.ClientQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("all-clients")]

        public async Task<ActionResult> GetAllClients([FromQuery]GetAllClientQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching all clients");
            
            var result = await mediator.Send(query, cancellationToken);
            logger.LogInformation("All clients fetched successfully");
            return Ok(result);
        }

        [HttpPost("update-client")]

        public async Task<ActionResult> UpdateClient([FromBody] UpdateClientCommand client, CancellationToken cancellationToken)
        {
            logger.LogInformation("Client update started");
            var result = await mediator.Send(client, cancellationToken);
            logger.LogInformation("Client update completed");
            return Ok(result);
        }
    }
}
