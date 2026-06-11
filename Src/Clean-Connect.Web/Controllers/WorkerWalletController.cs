using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("worker/wallet")]
public sealed class WorkerWalletController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(portal.GetWorkerWallet());
}
