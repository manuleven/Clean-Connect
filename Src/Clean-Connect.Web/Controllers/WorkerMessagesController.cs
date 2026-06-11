using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("worker/messages")]
public sealed class WorkerMessagesController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("Index", portal.GetWorkerMessages());
}
