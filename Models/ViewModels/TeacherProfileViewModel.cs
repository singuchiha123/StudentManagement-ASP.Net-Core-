using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models.ViewModels
{
    public class TeacherProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(50)]
        public string Subject { get; set; } = string.Empty;
    }
}