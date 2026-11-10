namespace StudentManagement.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public IEnumerable<Teacher> Teachers { get; set; } = new List<Teacher>();
        public IEnumerable<Class> Classes { get; set; } = new List<Class>();
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
    }
}

