using System;
using System.Collections.Generic;

namespace ApplicationTrackingSystem.Core.DTOs
{
    public class CreateApplicationRequest
    {
        public int JobRoleId { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateApplicationStatusRequest
    {
        public int ApplicationId { get; set; }
        public int NewStatus { get; set; }
        public string Comment { get; set; }
    }

    public class ApplicationResponse
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string JobRoleTitle { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Notes { get; set; }
        public List<ActivityLogResponse> ActivityLogs { get; set; }
    }

    public class ActivityLogResponse
    {
        public int Id { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public string Comment { get; set; }
        public string UpdatedByRole { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
