using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Student
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string Name {get; set;} = string.Empty;
        
        [Required]
        [Range(15, 100)]
        public int Age {get; set;}

        [Required]
        [EmailAddress]
        public string Email {get;set;} = string.Empty;

        [Required]
        [StringLength(50)]
        public string Major {get; set;} = string.Empty;

        // Foreign Key
        [Range(1, int.MaxValue, ErrorMessage = "Please select a teacher.")]
        public int TeacherId {get; set;}

        // Navigation Property
        [ValidateNever]
        public Teacher Teacher {get; set;} = null!;

        public int? ClassId {get; set;}

        public Class? Class {get; set;} = null!;

        public ICollection<Course> Courses {get; set;} = new List<Course>();
    }
}