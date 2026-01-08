using FinovaERP.Domain.Models.Entities;

namespace FinovaERP.Application.Interfaces.Services
{
    /// <summary>
    /// Session management service interface for user session handling
    /// </summary>
    public interface ISessionService
    {
        Task<UserSession> CreateSessionAsync(int userId, int companyId);
        Task<UserSession?> GetSessionAsync(string sessionId);
        Task<bool> ValidateSessionAsync(string sessionId);
        Task<bool> EndSessionAsync(string sessionId);
        Task<bool> EndAllUserSessionsAsync(int userId);
        Task CleanupExpiredSessionsAsync();
    }

    /// <summary>
    /// User session information
    /// </summary>
    public class UserSession
    {
        public string SessionId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Dictionary<string, object> SessionData { get; set; } = new Dictionary<string, object>();
    }
}
