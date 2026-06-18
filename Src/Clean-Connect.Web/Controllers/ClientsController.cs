using AspNetCoreHero.ToastNotification.Abstractions;
using Clean_Connect.Application.Command.ClientCommands;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Authorize]
public class ClientsController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMediator _mediator;
    private readonly INotyfService _notyf;
    private readonly UserManager<ApplicationUser> _user;

    public ClientsController(ILogger<AccountController> logger, IMediator mediator, INotyfService notyf)
    {
        _logger = logger;
        _mediator = mediator;
        _notyf = notyf;
        
    }

    [Authorize(Roles = "Admin, Client")]
    [HttpGet("Client-Profile")]
    public async Task<IActionResult> CreateClientProfile(CancellationToken cancellation)
    {
        _logger.LogInformation("Create client view");
        return View();
    }

    [Authorize(Roles = "Admin, Client")]
    [HttpGet("Client-Profile")]

    public async Task<IActionResult> CreateClientProfile([FromForm]CreateClientCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return View(result);
    }
}
