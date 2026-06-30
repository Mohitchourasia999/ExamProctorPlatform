using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Admin")] // Strict security checkpoint
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // 1. READ: List all questions and show Telemetry Reports
        public IActionResult Index()
        {
            var questions = _context.Questions.ToList();

            // Student metrics and proctoring tracking list pass karna
            ViewBag.Sessions = _context.ExamSessions.ToList();
            return View(questions);
        }

        // 2. CREATE: Get Form Page
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE: Post Form Data Submission
        [HttpPost]
        public IActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Questions.Add(question);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // 4. DELETE: Remove Question from Bank
        public IActionResult Delete(int id)
        {
            var question = _context.Questions.Find(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}