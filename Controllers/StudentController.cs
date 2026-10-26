using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // READ
        public IActionResult Index()
        {
            var students = _context.Students
                .Include(s => s.Teacher)
                .Include(s => s.Class)
                .Include(s => s.Courses)
                .ToList();

            return View(students);
        }


        // CREATE - Show form
        [HttpGet]
        public IActionResult Create()
        {
            LoadTeachers();
            LoadClasses();
            LoadCourses();

            return View();
        }


        // CREATE - Save
        [HttpPost]
        public IActionResult Create(
            Student student,
            int[] courseIds)
        {
            if (!ModelState.IsValid)
            {
                LoadTeachers(student.TeacherId);
                LoadClasses(student.ClassId);
                LoadCourses(courseIds);

                return View(student);
            }

            // Find selected courses
            if (courseIds != null && courseIds.Length > 0)
            {
                student.Courses = _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .ToList();
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // EDIT - Show form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _context.Students
                .Include(s => s.Courses)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            LoadTeachers(student.TeacherId);
            LoadClasses(student.ClassId);

            var selectedCourseIds = student.Courses
                .Select(c => c.Id)
                .ToArray();

            LoadCourses(selectedCourseIds);

            return View(student);
        }


        // EDIT - Save
        [HttpPost]
        public IActionResult Edit(
            Student student,
            int[] courseIds)
        {
            if (!ModelState.IsValid)
            {
                LoadTeachers(student.TeacherId);
                LoadClasses(student.ClassId);
                LoadCourses(courseIds);

                return View(student);
            }

            // Get the existing student with courses
            var existingStudent = _context.Students
                .Include(s => s.Courses)
                .FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Update normal student fields
            existingStudent.Name = student.Name;
            existingStudent.Age = student.Age;
            existingStudent.Email = student.Email;
            existingStudent.Major = student.Major;
            existingStudent.TeacherId = student.TeacherId;
            existingStudent.ClassId = student.ClassId;

            // Remove old course relationships
            existingStudent.Courses.Clear();

            // Add new course relationships
            if (courseIds != null && courseIds.Length > 0)
            {
                var selectedCourses = _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .ToList();

                foreach (var course in selectedCourses)
                {
                    existingStudent.Courses.Add(course);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // DELETE - Show confirmation
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var student = _context.Students
                .Include(s => s.Teacher)
                .Include(s => s.Class)
                .Include(s => s.Courses)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        // DELETE - Actually delete
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Load teachers
        private void LoadTeachers(int? selectedTeacherId = null)
        {
            ViewBag.Teachers = new SelectList(
                _context.Teachers
                    .OrderBy(t => t.Name)
                    .ToList(),
                "Id",
                "Name",
                selectedTeacherId
            );
        }


        // Load classes
        private void LoadClasses(int? selectedClassId = null)
        {
            ViewBag.Classes = new SelectList(
                _context.Classes
                    .OrderBy(c => c.Name)
                    .ToList(),
                "Id",
                "Name",
                selectedClassId
            );
        }


        // Load courses
        private void LoadCourses(int[]? selectedCourseIds = null)
        {
            ViewBag.Courses = new MultiSelectList(
                _context.Courses
                    .OrderBy(c => c.Code)
                    .ToList(),
                "Id",
                "Name",
                selectedCourseIds
            );
        }
    }
}