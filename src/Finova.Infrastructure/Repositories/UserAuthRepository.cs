using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Finova.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Repositories
{
    public sealed class UserAuthRepository
    {
        private readonly SqlConnectionFactory _factory;

        public UserAuthRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<(bool Found, Guid UserId, string UserName, string DisplayName, bool IsLocked, byte[] Salt, byte[] Hash, string Algo)> GetUserAsync(
            string userName, CancellationToken ct)
        {
            const string sql = @"
SELECT TOP(1)
  UserId, UserName, ISNULL(DisplayName, UserName) AS DisplayName,
  IsLocked, PasswordSalt, PasswordHash, PasswordAlgo
FROM sec.[User]
WHERE UserName = @UserName;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@UserName", userName);

            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
            if (!await r.ReadAsync(ct))
                return (false, Guid.Empty, "", "", false, Array.Empty<byte>(), Array.Empty<byte>(), "");

            var userId = r.GetGuid(0);
            var uName = r.GetString(1);
            var display = r.GetString(2);
            var isLocked = r.GetBoolean(3);
            var salt = (byte[])r["PasswordSalt"];
            var hash = (byte[])r["PasswordHash"];
            var algo = r.GetString(6);

            return (true, userId, uName, display, isLocked, salt, hash, algo);
        }

        public static bool VerifyPassword(string algo, byte[] salt, byte[] storedHash, string password)
        {
            // DB seed uses: HASHBYTES('SHA2_256', @Salt + CONVERT(VARBINARY(MAX), @Pwd))
            // NVARCHAR -> VARBINARY uses UTF-16LE in SQL Server => Encoding.Unicode
            if (!string.Equals(algo, "SHA2_256", StringComparison.OrdinalIgnoreCase))
                return false;

            var pwdBytes = Encoding.Unicode.GetBytes(password ?? string.Empty);

            byte[] input = new byte[salt.Length + pwdBytes.Length];
            Buffer.BlockCopy(salt, 0, input, 0, salt.Length);
            Buffer.BlockCopy(pwdBytes, 0, input, salt.Length, pwdBytes.Length);

            byte[] computed = SHA256.HashData(input); // 32 bytes

            // storedHash is VARBINARY(64) but likely contains 32 bytes + zeros.
            // Compare against the first 32 bytes of storedHash.
            if (storedHash.Length >= computed.Length)
            {
                for (int i = 0; i < computed.Length; i++)
                {
                    if (storedHash[i] != computed[i]) return false;
                }
                return true;
            }

            // fallback exact compare
            if (storedHash.Length != computed.Length) return false;
            for (int i = 0; i < computed.Length; i++)
            {
                if (storedHash[i] != computed[i]) return false;
            }
            return true;
        }
    }
}
