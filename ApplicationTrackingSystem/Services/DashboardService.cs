using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.Core.DTOs;
using ApplicationTrackingSystem.Core.Entities;
using ApplicationTrackingSystem.Core.Interfaces;
using ApplicationTrackingSystem.Infrastructure.Data;

namespace ApplicationTrackingSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResponse> GetDashboardDataAsync(int userId, string userRole)
        {
            IQueryable<Application> query = _context.Applications
                .Include(a => a.JobRole)
                .Include(a => a.Applicant)
                .Include(a => a.ActivityLogs);

            if (userRole == "Applicant")
            {
                query = query.Where(a => a.ApplicantId == userId);
            }
            else if (userRole == "BotMimic")
            {
                query = query.Where(a => a.JobRole.Type == RoleType.Technical);
            }
            else if (userRole == "Admin")
            {
                query = query.Where(a => a.JobRole.Type == RoleType.NonTechnical);
            }

            var applications = await query.ToListAsync();

            var dashboard = new DashboardResponse
            {
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == ApplicationStatus.Applied),
                ReviewedApplications = applications.Count(a => a.Status == ApplicationStatus.Reviewed),
                InterviewApplications = applications.Count(a => a.Status == ApplicationStatus.Interview),
                OfferApplications = applications.Count(a => a.Status == ApplicationStatus.Offer),
                RejectedApplications = applications.Count(a => a.Status == ApplicationStatus.Rejected),
                AcceptedApplications = applications.Count(a => a.Status == ApplicationStatus.Accepted),

                ApplicationsByRole = applications
                    .GroupBy(a => a.JobRole.Title)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ApplicationsByStatus = applications
                    .GroupBy(a => a.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),

                RecentActivities = applications
                    .SelectMany(a => a.ActivityLogs.Select(al => new
                    {
                        Application = a,
                        ActivityLog = al
                    }))
                    .OrderByDescending(x => x.ActivityLog.Timestamp)
                    .Take(10)
                    .Select(x => new RecentActivity
                    {
                        ApplicationTitle = $"{x.Application.Applicant.Username} - {x.Application.JobRole.Title}",
                        Status = x.ActivityLog.NewStatus.ToString(),
                        UpdatedBy = x.ActivityLog.UpdatedByRole,
                        Timestamp = x.ActivityLog.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList()
            };

            return dashboard;
        }
    }
}
