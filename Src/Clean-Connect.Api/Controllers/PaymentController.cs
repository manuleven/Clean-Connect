using Clean_Connect.Application.Command.PaymentCommand;
using Clean_Connect.Application.Command.WebhookCommand;
using Clean_Connect.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize([FromBody] InitializePaymentCommand command, CancellationToken cancellationToken)
        {
            var url = await mediator.Send(command, cancellationToken);
            logger.LogInformation(
                "Payment initialized for BookingId: {BookingId}, Amount: {Amount}, Email: {Email}, PaymentMethod: {PaymentMethod}",
                command.BookingId,
                command.Amount,
                command.Email,
                command.PaymentMethod);

            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("bookings/{bookingId:guid}/pay")]
        public async Task<IActionResult> PayForAcceptedBooking(Guid bookingId, [FromBody] ClientPaymentRequest request, CancellationToken cancellationToken)
        {
            var command = new PayForAcceptedBookingCommand(
                bookingId,
                request.ClientId,
                request.Email,
                request.PaymentMethod,
                request.CreatedBy);

            var result = await mediator.Send(command, cancellationToken);

            logger.LogInformation(
                "Payment initialized for accepted booking {BookingId} by client {ClientId}",
                bookingId,
                request.ClientId);

            return Ok(result);
        }

        [HttpPost("paystack/webhook")]
        public async Task<IActionResult> PaystackWebhook(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            logger.LogInformation("Received Paystack webhook with headers: {Headers}", Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            var payload = await reader.ReadToEndAsync(cancellationToken);
            logger.LogInformation("Webhook payload: {Payload}", payload);

            var signature = Request.Headers["x-paystack-signature"].ToString();
            logger.LogInformation("Received Paystack webhook with signature: {Signature}", signature);
            if (string.IsNullOrEmpty(signature))
            {
                logger.LogWarning("Missing signature header in Paystack webhook");
                return BadRequest("Missing signature header.");
            }

            var command = new ProcessPaystackWebhookCommand(payload, signature);

            await mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}
