using Clean_Connect.Application.Query.WorkersQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IMediator mediator;

        public WalletController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("workers/{workerId:guid}")]
        public async Task<IActionResult> GetWorkerWallet(Guid workerId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetWorkerWalletQuery(workerId), cancellationToken);
            return Ok(result);
        }

        [HttpGet("escrows/bookings/{bookingId:guid}")]
        public async Task<IActionResult> GetBookingEscrow(Guid bookingId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetBookingEscrowQuery(bookingId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
