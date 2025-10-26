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
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ApplicationService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ApplicationResponse> CreateApplicationAsync(int applicantId, CreateApplicationRequest request)
        {
            var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(request.JobRoleId);
            if (jobRole == null || !jobRole.IsActive)
            {
                throw new KeyNotFoundException("Job role not found or inactive");
            }

            var application = new Core.Entities.Application
            {
                ApplicantId = applicantId,
                JobRoleId = request.JobRoleId,
                Status = ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                Notes = request.Notes
            };

            await _unitOfWork.Applications.AddAsync(application);
            await _unitOfWork.CompleteAsync();

            var activityLog = new ActivityLog
            {
                ApplicationId = application.Id,
                UpdatedById = applicantId,
                PreviousStatus = ApplicationStatus.Applied,
                NewStatus = ApplicationStatus.Applied,
                Comment = "Application submitted",
                UpdatedByRole = "Applicant",
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(activityLog);
            await _unitOfWork.CompleteAsync();

            return await GetApplicationByIdAsync(application.Id, applicantId, "Applicant");
        }

        public async Task<ApplicationResponse> GetApplicationByIdAsync(int applicationId, int userId, string userRole)
        {
            var application = await _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.JobRole)
                .Include(a => a.ActivityLogs)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Application not found");
            }

            if (userRole == "Applicant" && application.ApplicantId != userId)
            {
                throw new UnauthorizedAccessException("You can only view your own applications");
            }

            return MapToApplicationResponse(application);
        }

        public async Task<IEnumerable<ApplicationResponse>> GetMyApplicationsAsync(int userId, string userRole)
        {
            IQueryable<Core.Entities.Application> query = _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.JobRole)
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
            return applications.Select(MapToApplicationResponse).ToList();
        }

        public async Task<ApplicationResponse> UpdateApplicationStatusAsync(int userId, string userRole, UpdateApplicationStatusRequest request)
        {
            var application = await _context.Applications
                .Include(a => a.JobRole)
                .Include(a => a.Applicant)
                .Include(a => a.ActivityLogs)
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Application not found");
            }

            if (userRole == "Admin" && application.JobRole.Type == RoleType.Technical)
            {
                throw new UnauthorizedAccessException("Admin can only update non-technical role applications");
            }

            if (userRole == "BotMimic" && application.JobRole.Type == RoleType.NonTechnical)
            {
                throw new UnauthorizedAccessException("Bot Mimic can only update technical role applications");
            }

            if (userRole == "Applicant")
            {
                throw new UnauthorizedAccessException("Applicants cannot update application status");
            }

            if (!Enum.IsDefined(typeof(ApplicationStatus), request.NewStatus))
            {
                throw new ArgumentException("Invalid application status");
            }

            var previousStatus = application.Status;
            application.Status = (ApplicationStatus)request.NewStatus;
            application.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Applications.UpdateAsync(application);

            var activityLog = new ActivityLog
            {
                ApplicationId = application.Id,
                UpdatedById = userId,
                PreviousStatus = previousStatus,
                NewStatus = application.Status,
                Comment = request.Comment ?? "Status updated",
                UpdatedByRole = userRole,
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(activityLog);
            await _unitOfWork.CompleteAsync();

            return await GetApplicationByIdAsync(application.Id, userId, userRole);
        }

        private ApplicationResponse MapToApplicationResponse(Core.Entities.Application application)
        {
            return new ApplicationResponse
            {
                Id = application.Id,
                ApplicantName = application.Applicant?.Username ?? "Unknown",
                JobRoleTitle = application.JobRole?.Title ?? "Unknown",
                Status = application.Status.ToString(),
                AppliedAt = application.AppliedAt,
                LastUpdatedAt = application.LastUpdatedAt,
                Notes = application.Notes,
                ActivityLogs = application.ActivityLogs?.Select(al => new ActivityLogResponse
                {
                    Id = al.Id,
                    PreviousStatus = al.PreviousStatus.ToString(),
                    NewStatus = al.NewStatus.ToString(),
                    Comment = al.Comment,
                    UpdatedByRole = al.UpdatedByRole,
                    Timestamp = al.Timestamp
                }).OrderByDescending(al => al.Timestamp).ToList() ?? new List<ActivityLogResponse>()
            };
        }
    }
}
