using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;
using ExamProctorPlatform.Models;
using System.Security.Claims;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // Main student dashboard listing active tests matching eligibility criteria
        public IActionResult Index()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            var student = _context.Users.Find(userId);

            // Hide exam layouts from listing if candidate doesn't match baseline criteria requirements
            var visibleSubjects = _context.SubjectConfigurations
                                         .Where(c => student!.CdacPercentage >= c.CutoffPercentage)
                                         .ToList();
            return View(visibleSubjects);
        }

        // Bypasses all scheduling restrictions to route directly to the active exam canvas
        public IActionResult VerifyExamAccess(string subject)
        {
            return RedirectToAction("TakeExam", new { subject = subject });
        }

        // Serves randomized problem entries and shuffles choices options dynamically
        public IActionResult TakeExam(string subject)
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            var session = _context.ExamSessions.FirstOrDefault(s => s.UserId == userId && s.Subject == subject && s.Status == "Active");
            if (session == null)
            {
                session = new ExamSession { UserId = userId, Subject = subject, Status = "Active", Score = 0 };
                _context.ExamSessions.Add(session);
                _context.SaveChanges();
            }

            if (session.Status == "Blocked_By_Violation") return RedirectToAction("Terminated");

            var questions = _context.Questions.Where(q => q.Subject == subject).ToList();
            var random = new Random();

            // Dual shuffling configuration matrix setup
            questions = questions.OrderBy(q => random.Next()).ToList(); // Shuffle question positions array

            foreach (var q in questions)
            {
                if (q.QuestionType == "Option" && q.Options != null && q.Options.Any())
                {
                    q.Options = q.Options.OrderBy(o => random.Next()).ToList(); // Shuffle nested choice indices
                }
            }

            ViewBag.SessionId = session.Id;
            ViewBag.Subject = subject;
            return View(questions);
        }

        // Processes input fields text tokens and calculates score values (+4 / -1 template metrics)
        [HttpPost]
        public IActionResult SubmitExam(IFormCollection form, int sessionId)
        {
            var session = _context.ExamSessions.Find(sessionId);
            if (session == null || session.Status != "Active") return BadRequest("Invalid session footprint context.");

            var questions = _context.Questions.Where(q => q.Subject == session.Subject).ToList();
            int finalScore = 0;

            foreach (var q in questions)
            {
                string submittedAns = form[$"question_{q.Id}"].ToString().Trim();
                session.LastQuestionProcessedId = q.Id;

                if (!string.IsNullOrEmpty(submittedAns))
                {
                    if (submittedAns.Equals(q.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        finalScore += q.PositiveMarks;
                    }
                    else
                    {
                        finalScore += q.NegativeMarks;
                    }
                }
            }

            session.Score = finalScore;
            session.Status = "Completed";
            _context.SaveChanges();

            return Ok(new { success = true });
        }

        public IActionResult Terminated()
        {
            return View();
        }
    }
}