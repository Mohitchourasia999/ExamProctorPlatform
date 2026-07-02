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

        [HttpGet]
        public IActionResult EditSubjectConfig(int id)
        {
            var subject = _context.SubjectConfigurations
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
                return NotFound();

            return View(subject);
        }

        [HttpPost]
        public IActionResult EditSubjectConfig(SubjectConfiguration subject)
        {
            if (!ModelState.IsValid)
                return View(subject);

            if (_context.SubjectConfigurations.Any(s =>
    s.SubjectName.ToLower() == subject.SubjectName.ToLower()
    && s.Id != subject.Id))
            {
                ModelState.AddModelError("", "A subject with this name already exists.");
                return View(subject);
            }

            // Fetch the existing subject from the database
            var existingSubject = _context.SubjectConfigurations
                .FirstOrDefault(s => s.Id == subject.Id);

            if (existingSubject == null)
                return NotFound();

            string oldSubjectName = existingSubject.SubjectName;

            // If the subject name has changed, update related tables
            if (oldSubjectName != subject.SubjectName)
            {
                var questions = _context.Questions
                    .Where(q => q.Subject == oldSubjectName)
                    .ToList();

                foreach (var question in questions)
                {
                    question.Subject = subject.SubjectName;
                }

                var sessions = _context.ExamSessions
                    .Where(s => s.Subject == oldSubjectName)
                    .ToList();

                foreach (var session in sessions)
                {
                    session.Subject = subject.SubjectName;
                }
            }

            // Update subject details
            existingSubject.SubjectName = subject.SubjectName;
            existingSubject.CutoffPercentage = subject.CutoffPercentage;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteSubjectConfig(int id)
        {
            var subject = _context.SubjectConfigurations
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
                return NotFound();

            var questions = _context.Questions
                .Where(q => q.Subject == subject.SubjectName)
                .ToList();

            _context.Questions.RemoveRange(questions);

            var sessions = _context.ExamSessions
                .Where(s => s.Subject == subject.SubjectName)
                .ToList();

            _context.ExamSessions.RemoveRange(sessions);

            _context.SubjectConfigurations.Remove(subject);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ViewQuestions(string subjectName)
        {
            var questions = _context.Questions
                .Where(q => q.Subject == subjectName)
                .ToList();

            ViewBag.Subject = subjectName;

            return View(questions);
        }

        [HttpGet]
        public IActionResult CreateQuestion(string subjectName)
        {
            var question = new Question
            {
                Subject = subjectName,
                PositiveMarks = 4,
                NegativeMarks = -1,
                QuestionType = "Option"
            };

            return View(question);
        }

        [HttpPost]
        public IActionResult CreateQuestion(Question question)
        {
            _context.Questions.Add(question);

            _context.SaveChanges();

            return RedirectToAction("ViewQuestions",
                new { subjectName = question.Subject });
        }

        [HttpGet]
        public IActionResult EditQuestion(int id)
        {
            var question = _context.Questions.FirstOrDefault(q => q.Id == id);

            if (question == null)
                return NotFound();

            return View(question);
        }

        [HttpPost]
        public IActionResult EditQuestion(Question question)
        {
            _context.Questions.Update(question);

            _context.SaveChanges();

            return RedirectToAction("ViewQuestions",
                new { subjectName = question.Subject });
        }

        [HttpGet]
        public IActionResult DeleteQuestion(int id)
        {
            var question = _context.Questions.FirstOrDefault(q => q.Id == id);

            if (question == null)
                return NotFound();

            string subject = question.Subject;

            _context.Questions.Remove(question);

            _context.SaveChanges();

            return RedirectToAction("ViewQuestions",
                new { subjectName = subject });
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