using Bakalarska_prace.Controllers;
using Bakalarska_prace.Models.ApplicationServices.Abstract;
using Bakalarska_prace.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bakalarska_prace.Areas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        ISecurityApplicationService _securityService;
        public AccountController(ISecurityApplicationService securityService)
        {
            _securityService = securityService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid == true)
            {
                bool isLogged = await _securityService.Login(loginVM);
                if (isLogged)
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", String.Empty), new { area = String.Empty });
                else
                    loginVM.LoginFailed = true;
            }

            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _securityService.Logout();
            return RedirectToAction(nameof(Login));
        }

    }
}
