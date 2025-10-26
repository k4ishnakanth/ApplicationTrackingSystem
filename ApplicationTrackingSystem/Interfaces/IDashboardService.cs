using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetDashboardDataAsync(int userId, string userRole);
    }
}
