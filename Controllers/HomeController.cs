using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var viewModel = new HomeViewModel
        {
            Students = _context.Students
                .Include(s => s.Teacher)
                .Include(s => s.Class)
                .Include(s => s.Courses)
                .ToList(),
            Teachers = _context.Teachers
                .Include(t => t.Class)
                .Include(t => t.Students)
                .ToList(),
            Classes = _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .ToList(),
            Courses = _context.Courses
                .Include(c => c.Students)
                .ToList()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
