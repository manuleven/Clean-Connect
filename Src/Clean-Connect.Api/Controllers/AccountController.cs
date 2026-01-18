using Clean_Connect.Application.Command.ApplicationUserCommand;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("register")]
        public async Task <ActionResult> Register([FromBody] RegisterUserCommand registerModel)
        {
          
            var response = await _mediator.Send(registerModel);
            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task <ActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailModel)
        {
            var response = await _mediator.Send(confirmEmailModel);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task <ActionResult> Login([FromBody] LoginCommand loginModel)
        {
            

            var response = await _mediator.Send(loginModel);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task <ActionResult> Logout()
        {
            var command = new LogOutCommand();
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
