using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.Core.Entities;
using ApplicationTrackingSystem.Core.Interfaces;
using ApplicationTrackingSystem.Infrastructure.Data;

namespace ApplicationTrackingSystem.Services
{
    public class BotMimicService : IBotMimicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();

        public BotMimicService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task ProcessTechnicalApplicationsAsync()
        {
            var applications = await _context.Applications
                .Include(a => a.JobRole)
                .Where(a => a.JobRole.Type == RoleType.Technical &&
                            a.Status != ApplicationStatus.Rejected &&
                            a.Status != ApplicationStatus.Accepted)
                .ToListAsync();

            foreach (var application in applications)
            {
                await ProcessApplicationWorkflowAsync(application);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task ProcessSpecificApplicationAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.JobRole)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Application not found");
            }

            if (application.JobRole.Type != RoleType.Technical)
            {
                throw new InvalidOperationException("Bot Mimic can only process technical role applications");
            }

            await ProcessApplicationWorkflowAsync(application);
            await _unitOfWork.CompleteAsync();
        }

        private async Task ProcessApplicationWorkflowAsync(Application application)
        {
            var previousStatus = application.Status;
            ApplicationStatus newStatus;
            string comment;

            switch (application.Status)
            {
                case ApplicationStatus.Applied:
                    newStatus = ApplicationStatus.Reviewed;
                    comment = "Application reviewed by automated system. Candidate profile matches job requirements.";
                    break;
                case ApplicationStatus.Reviewed:
                    if (_random.Next(100) < 80)
                    {
                        newStatus = ApplicationStatus.Interview;
                        comment = "Candidate shortlisted for interview. Technical assessment scores are satisfactory.";
                    }
                    else
                    {
                        newStatus = ApplicationStatus.Rejected;
                        comment = "Application rejected after review. Candidate does not meet minimum requirements.";
                    }
                    break;
                case ApplicationStatus.Interview:
                    if (_random.Next(100) < 60)
                    {
                        newStatus = ApplicationStatus.Offer;
                        comment = "Interview completed successfully. Offer package being prepared.";
                    }
                    else
                    {
                        newStatus = ApplicationStatus.Rejected;
                        comment = "Application rejected after interview. Technical skills did not meet expectations.";
                    }
                    break;
                case ApplicationStatus.Offer:
                    if (_random.Next(100) < 70)
                    {
                        newStatus = ApplicationStatus.Accepted;
                        comment = "Offer accepted by candidate. Onboarding process initiated.";
                    }
                    else
                    {
                        newStatus = ApplicationStatus.Rejected;
                        comment = "Candidate declined the offer.";
                    }
                    break;
                default:
                    return;
            }

            application.Status = newStatus;
            application.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Applications.UpdateAsync(application);

            var activityLog = new ActivityLog
            {
                ApplicationId = application.Id,
                UpdatedById = null,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                Comment = comment,
                UpdatedByRole = "Bot Mimic",
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(activityLog);
        }
    }
}
