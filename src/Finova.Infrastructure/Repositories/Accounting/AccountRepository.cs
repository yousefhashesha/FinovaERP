using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Accounting;
using Finova.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Repositories.Accounting
{
    public sealed class AccountRepository
    {
        private readonly SqlConnectionFactory _factory;
        public AccountRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<IReadOnlyList<AccountDto>> GetChartAsync(Guid companyId, bool includeInactive, CancellationToken ct)
        {
            const string sql = @"
SELECT CompanyId, AccountId, AccountCode, AccountName, AccountType, ParentAccountId,
       IsPosting, NormalBalance, [Level], IsActive
FROM acc.Account
WHERE CompanyId = @CompanyId
  AND IsDeleted = 0
  AND (@IncludeInactive = 1 OR IsActive = 1)
ORDER BY AccountCode;";

            var list = new List<AccountDto>();

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@IncludeInactive", includeInactive ? 1 : 0);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                list.Add(new AccountDto(
                    r.GetGuid(0),
                    r.GetGuid(1),
                    r.GetString(2),
                    r.GetString(3),
                    r.GetString(4),
                    r.IsDBNull(5) ? null : r.GetGuid(5),
                    r.GetBoolean(6),
                    r.GetString(7).Trim(),
                    r.GetByte(8),
                    r.GetBoolean(9)
                ));
            }

            return list;
        }

        public async Task<AccountDto?> GetByIdAsync(Guid companyId, Guid accountId, CancellationToken ct)
        {
            const string sql = @"
SELECT TOP(1) CompanyId, AccountId, AccountCode, AccountName, AccountType, ParentAccountId,
       IsPosting, NormalBalance, [Level], IsActive
FROM acc.Account
WHERE CompanyId = @CompanyId AND AccountId = @AccountId AND IsDeleted = 0;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@AccountId", accountId);

            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
            if (!await r.ReadAsync(ct)) return null;

            return new AccountDto(
                r.GetGuid(0),
                r.GetGuid(1),
                r.GetString(2),
                r.GetString(3),
                r.GetString(4),
                r.IsDBNull(5) ? null : r.GetGuid(5),
                r.GetBoolean(6),
                r.GetString(7).Trim(),
                r.GetByte(8),
                r.GetBoolean(9)
            );
        }

        public async Task<Guid> CreateAsync(Guid companyId, string code, string name, string type, Guid? parentId,
            bool isPosting, string normalBalance, byte level, Guid? userId, CancellationToken ct)
        {
            var id = Guid.NewGuid();

            const string sql = @"
INSERT INTO acc.Account
(CompanyId, AccountId, AccountCode, AccountName, AccountType, ParentAccountId, IsPosting, NormalBalance, [Level],
 IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES
(@CompanyId, @AccountId, @AccountCode, @AccountName, @AccountType, @ParentAccountId, @IsPosting, @NormalBalance, @Level,
 1, 0, SYSUTCDATETIME(), @UserId);";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@AccountId", id);
            cmd.Parameters.AddWithValue("@AccountCode", code);
            cmd.Parameters.AddWithValue("@AccountName", name);
            cmd.Parameters.AddWithValue("@AccountType", type);
            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)parentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsPosting", isPosting);
            cmd.Parameters.AddWithValue("@NormalBalance", normalBalance);
            cmd.Parameters.AddWithValue("@Level", level);
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
            return id;
        }

        public async Task UpdateAsync(Guid companyId, Guid accountId, string code, string name, string type, Guid? parentId,
            bool isPosting, string normalBalance, byte level, bool isActive, Guid? userId, CancellationToken ct)
        {
            const string sql = @"
UPDATE acc.Account
SET AccountCode=@AccountCode,
    AccountName=@AccountName,
    AccountType=@AccountType,
    ParentAccountId=@ParentAccountId,
    IsPosting=@IsPosting,
    NormalBalance=@NormalBalance,
    [Level]=@Level,
    IsActive=@IsActive,
    UpdatedAt=SYSUTCDATETIME(),
    UpdatedBy=@UserId
WHERE CompanyId=@CompanyId AND AccountId=@AccountId AND IsDeleted=0;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@AccountId", accountId);
            cmd.Parameters.AddWithValue("@AccountCode", code);
            cmd.Parameters.AddWithValue("@AccountName", name);
            cmd.Parameters.AddWithValue("@AccountType", type);
            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)parentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsPosting", isPosting);
            cmd.Parameters.AddWithValue("@NormalBalance", normalBalance);
            cmd.Parameters.AddWithValue("@Level", level);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
