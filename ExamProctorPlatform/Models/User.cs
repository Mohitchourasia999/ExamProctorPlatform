using System.ComponentModel.DataAnnotations;

namespace ExamProctorPlatform.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Student"; // "Admin" or "Student"

        [Range(0, 100, ErrorMessage = "CDAC percentage must be between 0 and 100.")]
        public double CdacPercentage { get; set; } = 0.0; // Student score tracking metric

        public string? ResetOtp { get; set; } // Temporary recovery tracking tokens
        public DateTime? OtpExpiry { get; set; }
    }
}