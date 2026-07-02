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
            // Get logged-in student ID
            var userIdStr = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            // Get current student
            var student = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (student == null)
                return RedirectToAction("Login", "Account");

            // Create exam session if not exists
            var session = _context.ExamSessions
                .FirstOrDefault(s => s.UserId == userId && s.Status == "Active");

            if (session == null)
            {
                session = new ExamSession
                {
                    UserId = userId,
                    Status = "Active",
                    Score = 0
                };

                _context.ExamSessions.Add(session);
                _context.SaveChanges();
            }

            // Blocked student
            if (session.Status == "Blocked_By_Violation")
            {
                return RedirectToAction("Terminated");
            }

            ViewBag.SessionId = session.Id;

            // Get subjects student is eligible for
            var availableSubjects = _context.SubjectConfigurations
                .Where(s => student.CdacPercentage >= s.CutoffPercentage)
                .ToList();

            return View(availableSubjects);
        }

        public IActionResult VerifyExamAccess(string subject)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            var student = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (student == null)
                return RedirectToAction("Login", "Account");

            var config = _context.SubjectConfigurations
                .FirstOrDefault(s => s.SubjectName == subject);

            if (config == null)
                return RedirectToAction("Index");

            if (student.CdacPercentage < config.CutoffPercentage)
            {
                TempData["Error"] = "You are not eligible for this exam.";
                return RedirectToAction("Index");
            }

            var session = _context.ExamSessions
                .FirstOrDefault(s =>
                    s.UserId == userId &&
                    s.Subject == subject &&
                    s.Status == "Active");

            if (session == null)
            {
                session = new ExamSession
                {
                    UserId = userId,
                    Subject = subject,
                    Status = "Active",
                    Score = 0
                };

                _context.ExamSessions.Add(session);
                _context.SaveChanges();
            }

            var questions = _context.Questions
                .Where(q => q.Subject == subject)
                .ToList();

            ViewBag.Subject = subject;
            ViewBag.SessionId = session.Id;

            return View("TakeExam", questions);
        }

        // Test auto termination error layout route
        public IActionResult Terminated()
        {
            return View();
        }
    }
}