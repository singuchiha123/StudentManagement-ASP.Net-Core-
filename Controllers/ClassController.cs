using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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