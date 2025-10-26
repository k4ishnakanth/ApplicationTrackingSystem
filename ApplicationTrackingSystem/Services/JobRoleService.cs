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
    public class JobRoleService : IJobRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public JobRoleService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<JobRoleResponse> CreateJobRoleAsync(int adminId, CreateJobRoleRequest request)
        {
            if (!Enum.IsDefined(typeof(RoleType), request.Type))
            {
                throw new ArgumentException("Invalid role type");
            }

            var jobRole = new JobRole
            {
                Title = request.Title,
                Description = request.Description,
                Type = (RoleType)request.Type,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedById = adminId
            };

            await _unitOfWork.JobRoles.AddAsync(jobRole);
            await _unitOfWork.CompleteAsync();

            return new JobRoleResponse
            {
                Id = jobRole.Id,
                Title = jobRole.Title,
                Description = jobRole.Description,
                Type = jobRole.Type.ToString(),
                IsActive = jobRole.IsActive,
                CreatedAt = jobRole.CreatedAt
            };
        }

        public async Task<IEnumerable<JobRoleResponse>> GetAllJobRolesAsync()
        {
            var jobRoles = await _context.JobRoles
                .Where(jr => jr.IsActive)
                .ToListAsync();

            return jobRoles.Select(jr => new JobRoleResponse
            {
                Id = jr.Id,
                Title = jr.Title,
                Description = jr.Description,
                Type = jr.Type.ToString(),
                IsActive = jr.IsActive,
                CreatedAt = jr.CreatedAt
            }).ToList();
        }

        public async Task<JobRoleResponse> GetJobRoleByIdAsync(int id)
        {
            var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(id);
            if (jobRole == null)
            {
                throw new KeyNotFoundException("Job role not found");
            }

            return new JobRoleResponse
            {
                Id = jobRole.Id,
                Title = jobRole.Title,
                Description = jobRole.Description,
                Type = jobRole.Type.ToString(),
                IsActive = jobRole.IsActive,
                CreatedAt = jobRole.CreatedAt
            };
        }
    }
}
