using Microsoft.EntityFrameworkCore;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<SubjectConfiguration> SubjectConfigurations { get; set; } // Added tracking table mapping hook
    }
}