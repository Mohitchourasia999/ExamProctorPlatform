using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;
using ExamProctorPlatform.Models;
using System.Security.Claims;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Student")] // Sirf Students ke liye entry gate
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // Exam Arena main screening feed
        public IActionResult Index()
        {
            // Current Logged-in Student ki ID extract karna session claim se
            var userIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            // Pehle check karein agar is student ka session database mein chal raha hai ya nahi
            var session = _context.ExamSessions.FirstOrDefault(s => s.UserId == userId && s.Status == "Active");

            if (session == null)
            {
                // Naya session allocate karna instant telemetry tracking ke liye
                session = new ExamSession { UserId = userId, Status = "Active", Score = 0 };
                _context.ExamSessions.Add(session);
                _context.SaveChanges();
            }

            if (session.Status == "Blocked_By_Violation")
            {
                return RedirectToAction("Terminated");
            }

            // Database se questions fetch karke live student view model par bhejna
            var questions = _context.Questions.ToList();
            ViewBag.SessionId = session.Id; // JavaScript connection pipeline link karne ke liye id backup

            return View(questions);
        }

        // Test auto termination error layout route
        public IActionResult Terminated()
        {
            return View();
        }
    }
}