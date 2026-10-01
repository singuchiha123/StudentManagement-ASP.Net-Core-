using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace StudentManagement.Models
{
    public class Course
    {
        public int Id {get; set;}

        [Required]
        [StringLength(50)]
        public string Name {get; set;} = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code {get; set;} = string.Empty;

        public ICollection<Student> Students { get; set; } = new List<Student>();

    }
}