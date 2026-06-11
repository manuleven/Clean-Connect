using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("worker/settings")]
public sealed class WorkerSettingsController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();
}
