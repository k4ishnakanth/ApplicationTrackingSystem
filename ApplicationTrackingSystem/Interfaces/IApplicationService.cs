using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IApplicationService
    {
        Task<ApplicationResponse> CreateApplicationAsync(int applicantId, CreateApplicationRequest request);
        Task<ApplicationResponse> GetApplicationByIdAsync(int applicationId, int userId, string userRole);
        Task<IEnumerable<ApplicationResponse>> GetMyApplicationsAsync(int userId, string userRole);
        Task<ApplicationResponse> UpdateApplicationStatusAsync(int userId, string userRole, UpdateApplicationStatusRequest request);
    }
}
