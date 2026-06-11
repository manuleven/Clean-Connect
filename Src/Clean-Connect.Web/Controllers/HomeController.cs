using Clean_Connect.Web.Models;
using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Clean_Connect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPortalExperienceService _portalExperience;

        public HomeController(ILogger<HomeController> logger, IPortalExperienceService portalExperience)
        {
            _logger = logger;
            _portalExperience = portalExperience;
        }

        public IActionResult Index()
        {
            return View(_portalExperience.GetPublicHome());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
