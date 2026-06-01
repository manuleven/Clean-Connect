using Clean_Connect.Web.Models;
using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Clean_Connect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExperienceDataService _experienceData;

        public HomeController(ILogger<HomeController> logger, IExperienceDataService experienceData)
        {
            _logger = logger;
            _experienceData = experienceData;
        }

        public IActionResult Index()
        {
            return View(_experienceData.GetHomePage());
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
