using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class SubjectConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        public double CutoffPercentage { get; set; }
    }
}