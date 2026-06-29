using AspNetCoreHero.ToastNotification.Abstractions;
using Clean_Connect.Application.Command.ApplicationUserCommand;
using Clean_Connect.Application.Command.Auth;
using Clean_Connect.Application.DTO;
using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers
{
    public class AuthController : Controller
    {

        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;
        private readonly INotyfService _notyf;
        private readonly UserManager<ApplicationUser> _user;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(ILogger<AuthController> logger, IMediator mediator, INotyfService notyf, UserManager<ApplicationUser> user, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _mediator = mediator;
            _notyf = notyf;
            _user = user;
            _signInManager = signInManager;
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login validation failed for {Email}", model.Email);
                _notyf.Error("Please correct the highlighted errors.");
                return View(model);
            }

            try
            {
                _logger.LogInformation("Login attempt for {Email}", model.Email);

                var command = new LoginCommand(
                    model.Email,
                    model.Password,
                    model.RememberMe);

                var result = await _mediator.Send(command, cancellationToken);

                // Email not confirmed
                if (result.EmailNotConfirmed == true)
                {
                    _logger.LogInformation(
                        "Unconfirmed email login attempt for {Email}",
                        model.Email);

                    _notyf.Warning(
                        "Your email address has not been verified.");

                    return RedirectToAction(
                        nameof(EmailNotConfirmed),
                        new { email = model.Email });
                }


                // Invalid login
                if (result == null || !result.IsSuccessful)
                {
                    _logger.LogWarning(
                        "Failed login attempt for {Email}. Reason: {Error}",
                        model.Email,
                        result?.ErrorMessage);

                    _notyf.Error(
                        result?.ErrorMessage ??
                        "Invalid email or password.");

                    ModelState.AddModelError(
                        string.Empty,
                        result?.ErrorMessage ??
                        "Invalid email or password.");

                    return View(model);
                }


                _logger.LogInformation(
                    "User {Email} logged in successfully",
                    model.Email);

                // Sign in the user to establish authentication cookie
                var appUser = await _user.FindByEmailAsync(model.Email);
                if (appUser != null)
                {
                    await _signInManager.SignInAsync(appUser, model.RememberMe);
                }

                if (result.RequiresWorkerProfileCompletion)
                {
                    return RedirectToAction("Create", "WorkerProfile");
                }

                //if (result.RequiresClientProfileCompletion)
                //{
                //    _logger.LogInformation("Redirecting to client profile creation.");
                //    return RedirectToAction(nameof(ClientsController.CreateClientProfile), "Clients");
                //}

                _notyf.Success("Login successful. Welcome back!");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while logging in {Email}",
                    model.Email);

                _notyf.Error(
                    "An unexpected error occurred. Please try again.");

                return View(model);
            }
        }
        [HttpGet("Register-User")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register-User")]
        public async Task<IActionResult> Register([FromForm] RegisterUserCommand request, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid)
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
                return RedirectToAction(nameof(PendingConfirmation));
            }
            _notyf.Error("Failed to register user.");
            return View("Error", null);
        }

        [HttpGet("PendingConfirmation")]
        public IActionResult PendingConfirmation(string email)
        {
            return View(model: email);
        }

        [HttpPost("Resend-Email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Resend confirmation email attempted with empty email.");
                _notyf.Warning("Invalid email address.");

                return RedirectToAction(nameof(EmailNotConfirmed));
            }

            try
            {
                var result = await _mediator.Send(new ResendEmailConfirmationCommand(email));

                if (result)
                {
                    _logger.LogInformation(
                        "Verification email resent successfully to {Email}",
                        email);

                    _notyf.Success(
                        "A new verification email has been sent. Please check your inbox.");

                    return RedirectToAction(nameof(EmailNotConfirmed), new { email });
                }

                _logger.LogWarning(
                    "Resend confirmation email failed for {Email}",
                    email);

                _notyf.Warning(
                    "Unable to resend verification email. The email may already be confirmed or you may need to wait before trying again.");

                return RedirectToAction(nameof(EmailNotConfirmed), new { email });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while resending confirmation email for {Email}",
                    email);

                _notyf.Error(
                    "Something went wrong while sending the verification email.");

                return View("Error");
            }
        }

        [HttpGet("Email-not-confirmed")]
        public IActionResult EmailNotConfirmed(string email)
        {
            _logger.LogInformation(
                "EmailNotConfirmed page accessed for {Email}",
                email); 



            return View("PendingConfirmation", model: email);
        }

        [HttpGet("Confirm-Email")]

        public async Task<IActionResult> ConfirmEmail(Guid userId, string token, CancellationToken cancellationToken)
        {
            var command = new ConfirmEmailCommand(userId.ToString(), token);

            var result = await _mediator.Send(command, cancellationToken);
            if (result)
            {
                _notyf.Success("Email confirmed successfully!");
                return View("ConfirmEmail");
            }
            _notyf.Error("Failed to confirm email.");
            return View("Error", null);
        }

        //[Authorize]
        //public async Task<IActionResult> GetStarted()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var user = await _user.FindByIdAsync(userId);

        //    if (await _user.IsInRoleAsync(user, "Client"))
        //    {
        //        var clientProfileExists = await _dbContext.ClientProfiles
        //            .AnyAsync(x => x.UserId == userId);

        //        if (!clientProfileExists)
        //        {
        //            return RedirectToAction(
        //                "Create",
        //                "ClientProfile");
        //        }

        //        return RedirectToAction(
        //            "Create",
        //            "Booking");
        //    }

        //    if (await _userManager.IsInRoleAsync(user, "Worker"))
        //    {
        //        var workerProfileExists = await _dbContext.WorkerProfiles
        //            .AnyAsync(x => x.UserId == userId);

        //        if (!workerProfileExists)
        //        {
        //            return RedirectToAction(
        //                "Create",
        //                "WorkerProfile");
        //        }

        //        return RedirectToAction(
        //            "Dashboard",
        //            "Worker");
        //    }

        //    return RedirectToAction("Index", "Home");
        //}
        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
    

