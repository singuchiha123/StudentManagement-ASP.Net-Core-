using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Password hashing service
builder.Services.AddScoped<IPasswordHasher<AppUser>,
    PasswordHasher<AppUser>>();

// Cookie Authentication
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed Admin account
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();

    var passwordHasher =
        services.GetRequiredService<IPasswordHasher<AppUser>>();

    var adminUsername =
        builder.Configuration["AdminAccount:UserName"];

    var adminPassword =
        builder.Configuration["AdminAccount:Password"];

    if (string.IsNullOrWhiteSpace(adminUsername) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new Exception("Admin account configuration is missing.");
    }

    var adminExists = await context.Users
        .AnyAsync(u => u.UserName == adminUsername);

    if (!adminExists)
    {
        var admin = new AppUser
        {
            UserName = adminUsername,
            Role = "Admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            adminPassword
        );

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();