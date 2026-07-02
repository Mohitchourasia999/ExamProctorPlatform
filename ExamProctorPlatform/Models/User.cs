using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Student"; // "Admin" or "Student"

        public double CdacPercentage { get; set; } = 0.0; // Student score tracking metric

        public string? ResetOtp { get; set; } // Temporary recovery tracking tokens
        public DateTime? OtpExpiry { get; set; }
    }
}