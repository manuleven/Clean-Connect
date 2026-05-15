using Clean_Connect.Application.Command.PaymentCommand;
using Clean_Connect.Application.DTO;
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
        private readonly ILogger<WalletController> logger;

        public WalletController(IMediator mediator, ILogger<WalletController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
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

        [HttpPost("workers/{workerId:guid}/request-payout")]
        public async Task<IActionResult> RequestWorkerPayout(Guid workerId, [FromBody] WorkerPayoutRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker {WorkerId} requesting payout for booking {BookingId}", workerId, request.BookingId);

            var command = new RequestPayoutCommand(
                request.BookingId,
                workerId,
                request.AccountNumber,
                request.BankCode,
                request.AccountName,
                request.Currency,
                request.ModifiedBy);

            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
