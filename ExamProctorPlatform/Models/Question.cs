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
        public string QuestionText { get; set; } = string.Empty;

        public string? OptionsJson { get; set; } // Database mein array JSON string bankar save hoga

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        public int Marks { get; set; } = 2;

        // Helper property: C# code mein dynamic List use karne ke liye
        [NotMapped]
        public List<string> Options
        {
            get => string.IsNullOrEmpty(OptionsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(OptionsJson) ?? new List<string>();
            set => OptionsJson = JsonSerializer.Serialize(value);
        }
    }
}