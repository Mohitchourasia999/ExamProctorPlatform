using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. POST: Register Submit
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Abhi bilkul simple raw storage rakh rahe hain testing aasan karne ke liye
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // 3. GET: Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 4. POST: Login Submit (Role-Based Cookie Generation)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                // User ke details ka security ticket (Claims) banana
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                // Browser mein encrypted session cookie store karna
                await HttpContext.SignInAsync("CookieAuth", principal);

                // Role ke hisab se redirection rules
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Question"); // Admin Dashboard
                }
                else
                {
                    return RedirectToAction("Index", "Student"); // Student Exam Portal
                }
            }

            ViewBag.Error = "Galat Email ya Password! Kripya sahi details dalein.";
            return View();
        }

        // 5. Logout Action
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            // Agar koi Student admin page kholne ki koshish karega toh ye badiya msg dikhayega
            return Content("🚫 Access Denied: Aapke paas is page ko dekhne ki permission nahi hai. Kripya Admin account se login karein.");
        }
    }
}