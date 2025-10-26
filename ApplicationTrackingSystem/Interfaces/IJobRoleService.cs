using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IJobRoleService
    {
        Task<JobRoleResponse> CreateJobRoleAsync(int adminId, CreateJobRoleRequest request);
        Task<IEnumerable<JobRoleResponse>> GetAllJobRolesAsync();
        Task<JobRoleResponse> GetJobRoleByIdAsync(int id);
    }
}
