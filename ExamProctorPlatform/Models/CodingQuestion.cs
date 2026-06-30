using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ExamProctorPlatform.Models
{
    public class CodingQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ProblemStatement { get; set; } = string.Empty;

        [Required]
        public string StarterCode { get; set; } = string.Empty;

        // Dynamic JSON Structure to store multiple test cases
        // Schema Layout: [{"InputA":7,"InputB":8,"Expected":"15"},{"InputA":10,"InputB":20,"Expected":"30"}]
        [Required]
        public string TestCasesJson { get; set; } = string.Empty;

        [NotMapped]
        public List<TestCaseBlueprint> TestCases
        {
            get => string.IsNullOrEmpty(TestCasesJson) ? new List<TestCaseBlueprint>() : JsonSerializer.Deserialize<List<TestCaseBlueprint>>(TestCasesJson) ?? new List<TestCaseBlueprint>();
            set => TestCasesJson = JsonSerializer.Serialize(value);
        }
    }

    public class TestCaseBlueprint
    {
        public int InputA { get; set; }
        public int InputB { get; set; }
        public string Expected { get; set; } = string.Empty;
    }
}