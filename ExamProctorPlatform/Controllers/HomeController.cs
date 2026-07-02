using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ExamProctorPlatform.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // If the user navigates out to the landing page, automatically terminate their session cookie
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("CookieAuth");
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Clean logout when hitting privacy policy from outside the dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("CookieAuth");
                return RedirectToAction("Privacy");
            }
            return View();
        }
    }
}