using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Data;

namespace ExamProctorPlatform.Controllers
{
    [Route("api/proctor")]
    [ApiController]
    public class ProctorApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProctorApiController(AppDbContext context)
        {
            _context = context;
        }

        // Increments the window violation counter from the exam popup window
        [HttpPost("log-violation")]
        public IActionResult LogViolation(string type, int sessionId)
        {
            var session = _context.ExamSessions.Find(sessionId);
            if (session == null)
            {
                return NotFound("Invalid test session identifier.");
            }

            if (type == "TabSwitch")
            {
                session.TabSwitches += 1;
            }

            // Lock out student if they exceed 3 window switches
            if (session.TabSwitches >= 3)
            {
                session.Status = "Blocked_By_Violation";
            }

            _context.SaveChanges();
            return Ok(new { currentSwitches = session.TabSwitches, status = session.Status });
        }
    }
}