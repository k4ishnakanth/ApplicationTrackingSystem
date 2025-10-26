using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;

namespace ApplicationTrackingSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        string GenerateJwtToken(int userId, string email, string role);
    }
}
