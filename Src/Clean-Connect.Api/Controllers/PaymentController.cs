using Clean_Connect.Application.Command.PaymentCommand;
using Clean_Connect.Application.Command.WebhookCommand;
using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Query.PaymentQuery;
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

        [HttpGet("payments/{paymentId:guid}/get-payment-by-id")]

        public async Task<IActionResult> GetPaymentById(Guid paymentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching payment for PaymentID: {PaymentID}", paymentId);

            var payment = new GetPaymentByIdCQuery(paymentId);

            var result = await mediator.Send(payment, cancellationToken).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet("Get-all-Payments")]

        public async Task<IActionResult> GetAllPayments(GetAllPaymentsQuery command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all payments operation started");
            var result = await mediator.Send(command, cancellationToken);

           return StatusCode(200, result);
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
            logger.LogInformation("");
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
