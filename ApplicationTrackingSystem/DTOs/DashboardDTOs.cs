using System.Collections.Generic;

namespace ApplicationTrackingSystem.Core.DTOs
{
    public class DashboardResponse
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ReviewedApplications { get; set; }
        public int InterviewApplications { get; set; }
        public int OfferApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public Dictionary<string, int> ApplicationsByRole { get; set; }
        public Dictionary<string, int> ApplicationsByStatus { get; set; }
        public List<RecentActivity> RecentActivities { get; set; }
    }

    public class RecentActivity
    {
        public string ApplicationTitle { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
        public string Timestamp { get; set; }
    }
}
