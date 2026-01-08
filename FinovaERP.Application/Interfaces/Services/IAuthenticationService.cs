using FinovaERP.Domain.Models.Entities;

namespace FinovaERP.Application.Interfaces.Services
{
    /// <summary>
    /// Authentication service interface for secure user authentication
    /// </summary>
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(string username, string password);
        Task<AuthenticationResult> LoginAsync(string username, string password, int companyId);
        Task LogoutAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        Task<User?> GetCurrentUserAsync();
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
    }

    /// <summary>
    /// Authentication result containing user information and tokens
    /// </summary>
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<int> Companies { get; set; } = new List<int>();
        public string? ErrorMessage { get; set; }
        public int? CompanyId { get; set; }
    }
}
