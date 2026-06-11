using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("client/settings")]
public sealed class ClientSettingsController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(portal.GetClientSettings());
}
