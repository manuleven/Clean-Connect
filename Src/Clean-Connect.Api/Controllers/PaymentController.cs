using Clean_Connect.Application.Command.PaymentCommand;
using Clean_Connect.Application.Command.WebhookCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<PaymentController> logger; 

        public PaymentController(IMediator _mediator, ILogger<PaymentController> _logger)
        {
            mediator = _mediator;
            logger = _logger;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize(InitializePaymentCommand command)
        {
            var url = await mediator.Send(command);
            logger.LogInformation("Payment initialized for BookingId: {BookingId}, Amount: {Amount}, Email: {Email}, PaymentMethod: {PaymentMethod}", 
                command.BookingId, command.Amount, command.Email, command.PaymentMethod);


            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("paystack/webhook")]
        public async Task<IActionResult> PaystackWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            logger.LogInformation("Received Paystack webhook with headers: {Headers}", Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            var payload = await reader.ReadToEndAsync();
            logger.LogInformation("Webhook payload: {Payload}", payload);

            var signature = Request.Headers["x-paystack-signature"]
                .ToString();
            logger.LogInformation("Received Paystack webhook with signature: {Signature}", signature);
            if (string.IsNullOrEmpty(signature))
            {
                logger.LogWarning("Missing signature header in Paystack webhook");
                return BadRequest("Missing signature header.");
            }


            var command = new ProcessPaystackWebhookCommand(
                payload,
                signature
            );

            await mediator.Send(command);

            return Ok();
        }
    }

}

