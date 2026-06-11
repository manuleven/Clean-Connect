using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

[Route("client/bookings")]
public sealed class ClientBookingsController(IPortalExperienceService portal) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(portal.GetClientBookingFlow());
}
