using FinovaERP.Application.Interfaces;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
using System.Linq.Expressions;

namespace FinovaERP.Application.Services
{
    /// <summary>
    /// User service implementation for user management operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IRepository<User> userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var users = await _userRepository.FindAsync(u => u.Username == username && u.IsActive);
            return users.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Validate username uniqueness
            if (!await IsUsernameUniqueAsync(user.Username))
                throw new InvalidOperationException(""Username already exists"");

            // Validate email uniqueness
            if (!await IsEmailUniqueAsync(user.Email))
                throw new InvalidOperationException(""Email already exists"");

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
            
            // Set creation metadata
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;
            user.FailedLoginAttempts = 0;
            user.IsLocked = false;

            var createdUser = await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return createdUser;
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Validate username uniqueness (excluding current user)
            var existingUser = await GetUserByUsernameAsync(user.Username);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new InvalidOperationException(""Username already exists"");

            // Validate email uniqueness (excluding current user)
            var usersWithEmail = await _userRepository.FindAsync(u => u.Email == user.Email && u.Id != user.Id);
            if (usersWithEmail.Any())
                throw new InvalidOperationException(""Email already exists"");

            // Update metadata
            user.ModifiedDate = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                user.ModifiedDate = DateTime.Now;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null || !user.IsActive || user.IsLocked)
                return false;

            return _passwordHasher.VerifyPassword(password, user.PasswordHash);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var existingUsers = await _userRepository.FindAsync(u => u.Username == username);
            return !existingUsers.Any();
        }

        private async Task<bool> IsEmailUniqueAsync(string email)
        {
            var existingUsers = await _userRepository.FindAsync(u => u.Email == email);
            return !existingUsers.Any();
        }
    }
}
