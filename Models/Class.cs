using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Class
    {
        public int Id {get; set;}

        [Required]
        [StringLength(50)]
        public string Name {get; set;} = string.Empty;

        public ICollection<Student> Students {get; set;} = new List<Student>();

        public ICollection<Teacher> Teachers {get; set;} = new List<Teacher>();
    }
}