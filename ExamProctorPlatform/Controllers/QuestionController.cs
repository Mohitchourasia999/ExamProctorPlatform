using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var configs = _context.SubjectConfigurations.ToList();
            return View(configs);
        }

        [HttpGet]
        public IActionResult CreateSubjectConfig()
        {
            return View();
        }

        // Processes the configuration record block and loops child items simultaneously
        [HttpPost]
        public IActionResult CreateSubjectConfig(SubjectConfiguration config, List<Question> Questions)
        {
            if (_context.SubjectConfigurations.Any(c => c.SubjectName.ToLower() == config.SubjectName.ToLower()))
            {
                ModelState.AddModelError("", "This subject blueprint code already exists.");
                return View(config);
            }

            // 1. Commit primary blueprint metadata settings row properties
            _context.SubjectConfigurations.Add(config);

            // 2. Safely cycle and serialize nested child problem nodes
            if (Questions != null && Questions.Any())
            {
                foreach (var q in Questions)
                {
                    if (!string.IsNullOrEmpty(q.QuestionText))
                    {
                        _context.Questions.Add(q);
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult SubjectResults(string subjectName)
        {
            ViewBag.Subject = subjectName;
            var sessions = _context.ExamSessions.Where(s => s.Subject == subjectName).ToList();
            return View(sessions);
        }
    }
}