using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Class> Classes { get; set; }

        public DbSet<Course> Courses { get; set; }

        // Authentication
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One AppUser has one Teacher profile
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Teacher)
                .WithOne()
                .HasForeignKey<AppUser>(u => u.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Prevent duplicate usernames
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // One Teacher has many Students
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Students)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Class has many Students
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Class has many Teachers
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Class)
                .WithMany(c => c.Teachers)
                .HasForeignKey(t => t.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student and Course: Many-to-Many
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students);
        }
    }
}