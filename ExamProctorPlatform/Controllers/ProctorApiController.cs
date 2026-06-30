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

        [HttpPost("log-violation")]
        public IActionResult LogViolation(string type, int sessionId)
        {
            var session = _context.ExamSessions.Find(sessionId);
            if (session == null) return NotFound("Session invalid tracking state");

            if (type == "TabSwitch") session.TabSwitches += 1;
            if (type == "MouseLeak") session.MouseLeaks += 1;
            if (type == "CopyAttempt") session.CopyAttempts += 1;

            // Strict policy rule validation parameter tracking
            if (session.TabSwitches >= 3)
            {
                session.Status = "Blocked_By_Violation";
            }

            _context.SaveChanges();
            return Ok(new { currentSwitches = session.TabSwitches, status = session.Status });
        }
    }
}
