using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Finova.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Repositories
{
    public sealed class PermissionRepository
    {
        private readonly SqlConnectionFactory _factory;

        public PermissionRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<IReadOnlySet<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
SELECT DISTINCT p.PermissionCode
FROM sec.UserRole ur
JOIN sec.RolePermission rp ON rp.RoleId = ur.RoleId
JOIN sec.Permission p ON p.PermissionId = rp.PermissionId
WHERE ur.UserId = @UserId
ORDER BY p.PermissionCode;";

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                set.Add(r.GetString(0));
            }

            return set;
        }
    }
}
