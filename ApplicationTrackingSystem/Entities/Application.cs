using System;
using System.Collections.Generic;

namespace ApplicationTrackingSystem.Core.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int JobRoleId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public User Applicant { get; set; }
        public JobRole JobRole { get; set; }
        public ICollection<ActivityLog> ActivityLogs { get; set; }
    }

    public enum ApplicationStatus
    {
        Applied = 1,
        Reviewed = 2,
        Interview = 3,
        Offer = 4,
        Rejected = 5,
        Accepted = 6
    }
}
