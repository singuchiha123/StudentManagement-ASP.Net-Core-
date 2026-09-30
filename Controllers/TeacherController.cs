using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        // Read
        public IActionResult Index()
        {
            var teachers = _context.Teachers
                .Include(t => t.Students)
                .ToList();

            return View(teachers);
        }

        // Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            if(ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            
            return View(teacher);
        }

        // Update
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if(teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost]
        public IActionResult Edit(Teacher teacher)
        {
            if(ModelState.IsValid)
            {
                _context.Teachers.Update(teacher);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // Delete
        public IActionResult Delete(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if(teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if(teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}