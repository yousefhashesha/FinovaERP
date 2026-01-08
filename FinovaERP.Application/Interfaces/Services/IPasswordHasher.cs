namespace FinovaERP.Application.Interfaces.Services
{
    /// <summary>
    /// Password hashing service interface for secure password management
    /// </summary>
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        bool IsPasswordStrong(string password);
    }
}
