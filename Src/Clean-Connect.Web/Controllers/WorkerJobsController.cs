using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("worker/jobs")]
public sealed class WorkerJobsController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(portal.GetWorkerJobs());
}
