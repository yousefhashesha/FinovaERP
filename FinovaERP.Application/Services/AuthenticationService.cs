using System.Text;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
using FinovaERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinovaERP.Application.Services
{
    /// <summary>
    /// Authentication service implementation with secure login functionality
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ISessionService _sessionService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Role> roleRepository,
            ISessionService sessionService,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _sessionService = sessionService;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationResult> LoginAsync(string username, string password)
        {
            try
            {
                // Find user by username
                var users = await _userRepository.FindAsync(u => u.Username == username && u.IsActive);
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    return new AuthenticationResult 
                    { 
                        Success = false, 
                        ErrorMessage = ""Invalid username or password"" 
                    };
                }

                // Check if account is locked
                if (user.IsLocked)
                {
                    return new AuthenticationResult 
                    { 
                        Success = false, 
                        ErrorMessage = ""Account is locked. Please contact administrator."" 
                    };
                }

                // Verify password
                if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    // Increment failed login attempts
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.IsLocked = true;
                    }
                    await _userRepository.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    return new AuthenticationResult 
                    { 
                        Success = false, 
                        ErrorMessage = ""Invalid username or password"" 
                    };
                }

                // Reset failed login attempts
                user.FailedLoginAttempts = 0;
                user.LastLoginDate = DateTime.Now;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Get user roles and companies
                var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == user.Id && ur.IsActive);
                var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();
                var companyIds = userRoles.Select(ur => ur.CompanyId).Distinct().ToList();

                var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id));
                var roleNames = roles.Select(r => r.Name).ToList();

                // Create session
                var primaryCompany = companyIds.FirstOrDefault();
                var session = await _sessionService.CreateSessionAsync(user.Id, primaryCompany);

                return new AuthenticationResult
                {
                    Success = true,
                    User = user,
                    Token = session.SessionId, // Using session ID as token for now
                    Roles = roleNames,
                    Companies = companyIds,
                    CompanyId = primaryCompany,
                    ExpiresAt = session.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    ErrorMessage = $""An error occurred during login: {ex.Message}"" 
                };
            }
        }

        public async Task<AuthenticationResult> LoginAsync(string username, string password, int companyId)
        {
            var result = await LoginAsync(username, password);
            
            if (result.Success && result.Companies.Contains(companyId))
            {
                result.CompanyId = companyId;
                return result;
            }
            
            if (result.Success)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    ErrorMessage = ""User does not have access to the specified company"" 
                };
            }
            
            return result;
        }

        public async Task LogoutAsync(int userId)
        {
            await _sessionService.EndAllUserSessionsAsync(userId);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return await _sessionService.ValidateSessionAsync(token);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            // This would typically get the current user from the session/token
            // Implementation depends on how you store the current session
            return null;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return false;

                // Verify current password
                if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                    return false;

                // Validate new password strength
                if (!_passwordHasher.IsPasswordStrong(newPassword))
                    return false;

                // Hash and update new password
                user.PasswordHash = _passwordHasher.HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return false;

                // Validate new password strength
                if (!_passwordHasher.IsPasswordStrong(newPassword))
                    return false;

                // Hash and update new password
                user.PasswordHash = _passwordHasher.HashPassword(newPassword);
                user.IsLocked = false;
                user.FailedLoginAttempts = 0;
                
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
