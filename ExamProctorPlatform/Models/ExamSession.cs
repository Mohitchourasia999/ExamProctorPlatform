using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class ExamSession
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        public int Score { get; set; }

        public int TabSwitches { get; set; } = 0;

        public int LastQuestionProcessedId { get; set; } = 0;

        public string Status { get; set; } = "Active";
    }
}