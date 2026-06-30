using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class ExamSession
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public int TabSwitches { get; set; } = 0;
        public int CopyAttempts { get; set; } = 0;
        public int MouseLeaks { get; set; } = 0;
        public string Status { get; set; } = "Active"; // Active, Completed, Blocked_By_Violation
    }
}