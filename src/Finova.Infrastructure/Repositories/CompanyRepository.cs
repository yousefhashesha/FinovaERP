using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Auth;
using Finova.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Repositories
{
    public sealed class CompanyRepository
    {
        private readonly SqlConnectionFactory _factory;

        public CompanyRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<IReadOnlyList<CompanyDto>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
SELECT c.CompanyId, c.CompanyCode, c.CompanyName, c.IsActive
FROM sec.UserCompany uc
JOIN core.Company c ON c.CompanyId = uc.CompanyId
WHERE uc.UserId = @UserId
ORDER BY uc.IsDefault DESC, c.CompanyName;";

            var list = new List<CompanyDto>();

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                list.Add(new CompanyDto(
                    r.GetGuid(0),
                    r.GetString(1),
                    r.GetString(2),
                    r.GetBoolean(3)
                ));
            }

            return list;
        }

        public async Task<CompanyDto?> GetDefaultCompanyAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
SELECT TOP(1) c.CompanyId, c.CompanyCode, c.CompanyName, c.IsActive
FROM sec.UserCompany uc
JOIN core.Company c ON c.CompanyId = uc.CompanyId
WHERE uc.UserId = @UserId
ORDER BY uc.IsDefault DESC, c.CompanyName;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
            if (!await r.ReadAsync(ct))
                return null;

            return new CompanyDto(r.GetGuid(0), r.GetString(1), r.GetString(2), r.GetBoolean(3));
        }
    }
}
