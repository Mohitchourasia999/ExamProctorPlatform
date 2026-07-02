using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class SubjectConfiguration
    {
        [Key]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        public double CutoffPercentage { get; set; } = 0.0;
    }
}