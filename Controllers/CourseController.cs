using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        // Read
        public IActionResult Index()
        {
            var courses = _context.Courses
                .Include(c => c.Students)
                .ToList();
            
            return View(courses);
        }

        // Create
        [HttpGet]
        public IActionResult Create()
        {   
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            if(!ModelState.IsValid)
            {
                return View();    
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);
            
            if(course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Courses.Update(course);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses
                .Include(c => c.Students)
                .FirstOrDefault(c => c.Id == id);

            if(course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Courses.Find(id);
            
            if(course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}