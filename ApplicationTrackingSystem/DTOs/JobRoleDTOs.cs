using System;

namespace ApplicationTrackingSystem.Core.DTOs
{
    public class CreateJobRoleRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; } // 1=Technical, 2=NonTechnical
    }

    public class JobRoleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
