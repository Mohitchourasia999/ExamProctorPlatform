using System;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // If a logged-in user leaves their dashboard to come here, log them out instantly
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("CookieAuth");
                return RedirectToAction("Register");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, double? studentPercentage)
        {
            if (user.Role == "Student")
            {
                user.CdacPercentage = studentPercentage ?? 0.0;
            }
            else
            {
                user.CdacPercentage = 0.0;
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // Force logout immediately if an authenticated user accesses the login screen
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("CookieAuth");
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

                return user.Role == "Admin" ? RedirectToAction("Index", "Question") : RedirectToAction("Index", "Student");
            }
            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult RequestResetOtp(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "Admin");
            if (user == null)
            {
                ViewBag.Error = "Admin account email not found.";
                return View("ForgotPassword");
            }

            string generatedOtp = new Random().Next(100000, 999999).ToString();
            user.ResetOtp = generatedOtp;
            user.OtpExpiry = DateTime.Now.AddMinutes(10);
            _context.SaveChanges();

            ViewBag.Email = email;
            ViewBag.Message = $"[SANDBOX RUNTIME SIMULATION] Security authorization code generated successfully: {generatedOtp}";

            return View("VerifyOtp");
        }

        [HttpPost]
        public IActionResult VerifyAndResetPassword(string email, string otp, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.ResetOtp == otp && u.Role == "Admin");
            if (user == null || user.OtpExpiry < DateTime.Now)
            {
                ViewBag.Error = "Invalid verification OTP code or expiry window passed.";
                ViewBag.Email = email;
                return View("VerifyOtp");
            }

            user.PasswordHash = newPassword;
            user.ResetOtp = null;
            user.OtpExpiry = null;
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}