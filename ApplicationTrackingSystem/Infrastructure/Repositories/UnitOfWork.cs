using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.Entities;
using ApplicationTrackingSystem.Core.Interfaces;
using ApplicationTrackingSystem.Infrastructure.Data;

namespace ApplicationTrackingSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<User> Users { get; }
        public IRepository<JobRole> JobRoles { get; }
        public IRepository<Application> Applications { get; }
        public IRepository<ActivityLog> ActivityLogs { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            JobRoles = new Repository<JobRole>(_context);
            Applications = new Repository<Application>(_context);
            ActivityLogs = new Repository<ActivityLog>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
