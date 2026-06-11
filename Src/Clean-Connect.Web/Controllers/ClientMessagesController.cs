using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("client/messages")]
public sealed class ClientMessagesController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("Index", portal.GetClientMessages());
}
