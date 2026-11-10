using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StudentManagement.Data;
using StudentManagement.Models;
using StudentManagement.Models.ViewModels;

namespace StudentManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AccountController(
            AppDbContext context,
            IPasswordHasher<AppUser> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // LOGIN - Show form
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        // LOGIN - Validate user
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid username or password."
                );

                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid username or password."
                );

                return View(model);
            }

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                new Claim(ClaimTypes.Name, user.UserName),

                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            // Sign in using authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties
            );

            if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // LOGOUT
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        // CREATE TEACHER ACCOUNT - Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(
            CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usernameExists = await _context.Users
                .AnyAsync(u => u.UserName == model.UserName);

            if (usernameExists)
            {
                ModelState.AddModelError(
                    "UserName",
                    "This username already exists."
                );

                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Role = "Teacher"
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                model.Password
            );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Teacher account created successfully.";

            return RedirectToAction("CreateUser");
        }

        // ACCESS DENIED
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}