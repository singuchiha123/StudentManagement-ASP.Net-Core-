using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StudentManagement.Models.ViewModels;

namespace StudentManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
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
                .Include(t => t.Class)
                .ToList();

            return View(teachers);
        }

        // Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction("CreateUser", "Account");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            if(ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);

                _context.SaveChanges();

                LoadClasses(teacher.ClassId);
                return RedirectToAction("Index");
            }
            
            return View(teacher);
        }

        // Update
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if(teacher == null)
            {
                return NotFound();
            }

            LoadClasses(teacher.ClassId);
            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Teacher teacher)
        {
            if(ModelState.IsValid)
            {
                _context.Teachers.Update(teacher);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadClasses(teacher.ClassId);
            return View(teacher);
        }

        // TEACHER: View own profile
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var user = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Teacher == null)
            {
                return View(new TeacherProfileViewModel());
            }

            var model = new TeacherProfileViewModel
            {
                Name = user.Teacher.Name,
                Email = user.Teacher.Email,
                Subject = user.Teacher.Subject
            };

            return View(model);
        }

        // TEACHER: Create or update own profile
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(
            TeacherProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var user = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Create profile if it doesn't exist
            if (user.Teacher == null)
            {
                var teacher = new Teacher
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject
                };

                _context.Teachers.Add(teacher);

                await _context.SaveChangesAsync();

                // Connect teacher profile to login account
                user.TeacherId = teacher.Id;
            }
            else
            {
                // Update existing profile
                user.Teacher.Name = model.Name;
                user.Teacher.Email = model.Email;
                user.Teacher.Subject = model.Subject;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your profile has been saved successfully.";

            return RedirectToAction("MyProfile");
        }

        private void LoadClasses(int? selectedClassId = null)
        {
            ViewBag.Classes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Classes.OrderBy(c => c.Name).ToList(),
                "Id",
                "Name",
                selectedClassId
            );
        }

        // Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if(teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        
        [Authorize(Roles = "Admin")]
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