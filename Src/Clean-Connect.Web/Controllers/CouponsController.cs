using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Connect.Web.Controllers;

public sealed class CouponsController(IExperienceDataService experienceData) : Controller
{
    public IActionResult Index()
    {
        return View(experienceData.GetCouponsPortal());
    }
}
