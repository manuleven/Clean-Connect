using AspNetCoreHero.ToastNotification.Abstractions;
using Clean_Connect.Application.Command.ApplicationUserCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;
        private readonly INotyfService _notyf;

        public AccountController(ILogger<AccountController> logger, IMediator mediator, INotyfService notyf)
        {
            _logger = logger;
            _mediator = mediator;
            _notyf = notyf;
        }   

        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("Register-User")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register-User")]
        public async Task<IActionResult> Register([FromForm] RegisterUserCommand request, CancellationToken cancellationToken)
        {

            if(!ModelState.IsValid)
            {
                _notyf.Error("Please correct the errors in the form.");
                return View(request);
            }
            var result = await _mediator.Send(request, cancellationToken);
            // Fix: 'result' is a Guid, not an object with 'Success' property.
            // Assume registration is successful if Guid is not empty.
            if (result != Guid.Empty)
            {
                _notyf.Success("User registered successfully!");
                return RedirectToAction(nameof(RegistrationSuccessful));
            }
            _notyf.Error("Failed to register user.");
            return View("Error", null);
        }

        [HttpGet("RegisterationSuccessful")]
        public IActionResult RegistrationSuccessful(string email)
        {
            return View(model : email);
        }

        [HttpGet("Confirm-Email")]

        public async Task<IActionResult> ConfirmEmail(Guid userId, string token, CancellationToken cancellationToken)
        {
            var command = new ConfirmEmailCommand(userId.ToString(), token);
            
            var result = await _mediator.Send(command, cancellationToken);
            if (result)
            {
                _notyf.Success("Email confirmed successfully!");
                return View("EmailConfirmation");
            }
            _notyf.Error("Failed to confirm email.");
            return View("Error", null);
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
