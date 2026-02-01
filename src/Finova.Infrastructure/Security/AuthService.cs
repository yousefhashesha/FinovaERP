using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Auth;
using Finova.Infrastructure.Repositories;

namespace Finova.Infrastructure.Security
{
    /// <summary>
    /// DB-backed authentication service (Phase 1.11).
    /// </summary>
    public sealed class AuthService : IAuthService
    {
        private readonly UserAuthRepository _repo;

        public AuthService(UserAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult> SignInAsync(string username, string password, CancellationToken ct = default)
        {
            username = (username ?? string.Empty).Trim();

            if (username.Length == 0 || string.IsNullOrWhiteSpace(password))
                return new AuthResult(false, "Username and password are required.");

            var u = await _repo.GetUserAsync(username, ct);
            if (!u.Found)
                return new AuthResult(false, "Invalid username or password.");

            if (u.IsLocked)
                return new AuthResult(false, "User is locked.");

            bool ok = UserAuthRepository.VerifyPassword(u.Algo, u.Salt, u.Hash, password);
            if (!ok)
                return new AuthResult(false, "Invalid username or password.");

            return new AuthResult(true, "OK", u.UserId, u.DisplayName);
        }
    }
}
