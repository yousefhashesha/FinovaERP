using System.Threading;
using System.Threading.Tasks;

namespace Finova.Application.Contracts.Auth
{
    public interface IAuthService
    {
        Task<AuthResult> SignInAsync(string username, string password, CancellationToken ct = default);
    }
}
