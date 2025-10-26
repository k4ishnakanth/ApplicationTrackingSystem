using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ApplicationTrackingSystem.Core.Entities
{
    public class JobRole
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RoleType Type { get; set; } // Technical or NonTechnical
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }

        // Navigation properties
        public User CreatedBy { get; set; }
        public ICollection<Application> Applications { get; set; }
    }

    public enum RoleType
    {
        Technical = 1,
        NonTechnical = 2
    }
}
