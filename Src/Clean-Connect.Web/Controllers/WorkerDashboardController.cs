using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("worker")]
public sealed class WorkerDashboardController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    [HttpGet("dashboard")]
    public IActionResult Index() => View(portal.GetWorkerDashboard());
}
