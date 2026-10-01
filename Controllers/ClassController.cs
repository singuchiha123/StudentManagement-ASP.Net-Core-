using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        // Index
        public IActionResult Index()
        {
            var classes = _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .ToList();

            return View(classes);
        }

        // Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Class @class)
        {
            if (!ModelState.IsValid)
            {
                return View(@class);
            }

            _context.Classes.Add(@class);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var @class = _context.Classes.Find(id);

            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        [HttpPost]
        public IActionResult Edit(Class @class)
        {
            if (!ModelState.IsValid)
            {
                return View(@class);
            }

            _context.Classes.Update(@class);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Delete
        public IActionResult Delete(int id)
        {
            var @class = _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .FirstOrDefault(c => c.Id == id);
            
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var @class = _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .FirstOrDefault(c => c.Id == id);

            if (@class == null)
            {
                return NotFound();
            }

            if (@class.Students.Any() || @class.Teachers.Any())
            {
                ModelState.AddModelError("", "Cannot delete a class that still has students or teachers.");

                return View(@class);
            }

            _context.Classes.Remove(@class);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}