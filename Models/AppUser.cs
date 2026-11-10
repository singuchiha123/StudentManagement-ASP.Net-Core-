using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Teacher";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Connect login account to Teacher profile
        public int? TeacherId { get; set; }

        public Teacher? Teacher { get; set; }
    }
}