using FinovaERP.Domain.Models.Entities;

namespace FinovaERP.Application.Interfaces.Services
{
    /// <summary>
    /// User service interface for user management
    /// </summary>
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> ValidateUserAsync(string username, string password);
        Task UpdateLastLoginAsync(int userId);
    }
}
