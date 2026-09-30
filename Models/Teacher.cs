using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Teacher
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string Name {get; set;} = string.Empty;

        [Required]
        [EmailAddress]
        public string Email {get; set;} = string.Empty;

        [Required]
        [StringLength(50)]
        public string Subject {get; set;} = string.Empty;

        // One teacher has many students
        public ICollection<Student> Students {get; set;} = new List<Student>();

        public int? ClassId {get; set;}
        
        public Class? Class {get; set;} = null!;
    }
}