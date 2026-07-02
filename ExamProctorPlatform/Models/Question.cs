using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ExamProctorPlatform.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string QuestionType { get; set; } = "Option";

        public string? OptionsJson { get; set; }

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        public int PositiveMarks { get; set; } = 4;
        public int NegativeMarks { get; set; } = -1;

        [NotMapped]
        public List<string> Options
        {
            get => string.IsNullOrEmpty(OptionsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(OptionsJson) ?? new List<string>();
            set => OptionsJson = JsonSerializer.Serialize(value);
        }
    }
}