using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.Core.Entities;

namespace ApplicationTrackingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<JobRole> JobRoles { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JobRole configuration
            modelBuilder.Entity<JobRole>()
                .HasOne(j => j.CreatedBy)
                .WithMany()
                .HasForeignKey(j => j.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Application configuration
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Applicant)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.JobRole)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Activity log configuration
            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.Application)
                .WithMany(a => a.ActivityLogs)
                .HasForeignKey(al => al.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.UpdatedBy)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed default users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@ats.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = UserRole.Admin, CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Username = "botmimic", Email = "bot@ats.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Bot@123"), Role = UserRole.BotMimic, CreatedAt = DateTime.UtcNow },
                new User { Id = 3, Username = "applicant", Email = "applicant@ats.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), Role = UserRole.Applicant, CreatedAt = DateTime.UtcNow }
            );

            // Seed default job roles
            modelBuilder.Entity<JobRole>().HasData(
                new JobRole { Id = 1, Title = "Backend Developer", Description = "Develops backend APIs", Type = RoleType.Technical, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedById = 1 },
                new JobRole { Id = 2, Title = "HR Manager", Description = "Handles HR tasks and recruiting", Type = RoleType.NonTechnical, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedById = 1 }
            );
        }
    }
}
