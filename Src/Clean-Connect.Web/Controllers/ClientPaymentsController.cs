using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("client/payments")]
public sealed class ClientPaymentsController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(portal.GetClientPayments());
}
