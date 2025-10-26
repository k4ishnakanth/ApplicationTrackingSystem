using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IBotMimicService
    {
        Task ProcessTechnicalApplicationsAsync();
        Task ProcessSpecificApplicationAsync(int applicationId);
    }
}
