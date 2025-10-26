using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.Entities;

using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<JobRole> JobRoles { get; }
        IRepository<Application> Applications { get; }
        IRepository<ActivityLog> ActivityLogs { get; }
        Task<int> CompleteAsync();
    }
}
