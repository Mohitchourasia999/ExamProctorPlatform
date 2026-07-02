using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }

        public int ExamSessionId { get; set; }

        public int QuestionId { get; set; }

        public string StudentAnswerText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int MarksAwarded { get; set; }
    }
}