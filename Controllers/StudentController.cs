using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

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
                .ToList();

            return View(students);
        }

        // CREATE - Show form
        [HttpGet]
        public IActionResult Create()
        {
            LoadTeachers();
            LoadClasses();
            return View();
        }

        // CREATE - Save
        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine(
                            $"{item.Key}: {error.ErrorMessage}"
                        );
                    }
                }

                LoadTeachers(student.TeacherId);
                LoadClasses(student.ClassId);
                return View(student);
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // UPDATE - Show form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            // Load teachers and select the student's current teacher
            LoadTeachers(student.TeacherId);
            LoadClasses(student.ClassId);
            return View(student);
        }

        // UPDATE - Save
        [HttpPost]
        public IActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Update(student);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Reload teachers when validation fails
            LoadTeachers(student.TeacherId);
            LoadClasses(student.ClassId);
            return View(student);
        }

        // Load teachers for Create/Edit dropdown
        private void LoadTeachers(int? selectedTeacherId = null)
        {
            ViewBag.Teachers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Teachers.OrderBy(t => t.Name).ToList(),
                "Id",
                "Name",
                selectedTeacherId
            );
        }

        // Load classes
        private void LoadClasses(int? selectedClassId = null)
        {
            ViewBag.Classes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Classes.OrderBy(c => c.Name).ToList(),
                "Id",
                "Name",
                selectedClassId
            );
        }

        // DELETE - Show confirmation
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);

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
    }
}