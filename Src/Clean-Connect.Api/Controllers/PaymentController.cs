using Clean_Connect.Application.Command.PaymentCommand;
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

        public PaymentController(IMediator _mediator)
        {
            mediator = _mediator;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize(InitializePaymentCommand command)
        {
            var url = await mediator.Send(command);
            return Ok(new { checkoutUrl = url });
        }
    }

}

