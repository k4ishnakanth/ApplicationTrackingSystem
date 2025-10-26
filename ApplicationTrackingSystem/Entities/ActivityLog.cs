using System;

namespace ApplicationTrackingSystem.Core.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int? UpdatedById { get; set; }
        public ApplicationStatus PreviousStatus { get; set; }
        public ApplicationStatus NewStatus { get; set; }
        public string Comment { get; set; }
        public string UpdatedByRole { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public Application Application { get; set; }
        public User UpdatedBy { get; set; }
    }
}
