using Bakalarska_prace.Areas.Security.Controllers;
using Bakalarska_prace.Models;
using Bakalarska_prace.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bakalarska_prace.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager) + ", " + nameof(Roles.Employee))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return RedirectToAction(nameof(HomeController.Login), nameof(AccountController).Replace("Controller", String.Empty), new { area = "Security" });
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