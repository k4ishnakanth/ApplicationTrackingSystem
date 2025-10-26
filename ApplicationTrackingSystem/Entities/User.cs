using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ApplicationTrackingSystem.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public ICollection<Application> Applications { get; set; }
        public ICollection<ActivityLog> ActivityLogs { get; set; }
    }

    public enum UserRole
    {
        Applicant = 1,
        BotMimic = 2,
        Admin = 3
    }
}
