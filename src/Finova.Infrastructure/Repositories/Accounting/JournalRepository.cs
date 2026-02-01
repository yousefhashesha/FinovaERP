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
    public sealed class JournalRepository
    {
        private readonly SqlConnectionFactory _factory;
        public JournalRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<IReadOnlyList<JournalDto>> GetJournalsAsync(Guid companyId, CancellationToken ct)
        {
            const string sql = @"
SELECT CompanyId, JournalId, JournalCode, JournalName, IsActive
FROM acc.Journal
WHERE CompanyId=@CompanyId AND IsDeleted=0 AND IsActive=1
ORDER BY JournalCode;";

            var list = new List<JournalDto>();

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                list.Add(new JournalDto(r.GetGuid(0), r.GetGuid(1), r.GetString(2), r.GetString(3), r.GetBoolean(4)));
            }
            return list;
        }

        public async Task<Guid> InsertHeaderAsync(Guid companyId, Guid userId, JournalHeaderDto h, CancellationToken ct)
        {
            const string sql = @"
INSERT INTO acc.JournalHeader
(CompanyId, JournalHeaderId, JournalId, JeNo, JeDate, Description, Status,
 SourceModule, SourceType, SourceDocumentId,
 FiscalYearId, FiscalPeriodId, TotalDebit, TotalCredit,
 PostedAt, PostedBy, IsDeleted, CreatedAt, CreatedBy)
VALUES
(@CompanyId, @JournalHeaderId, @JournalId, @JeNo, @JeDate, @Description, @Status,
 @SourceModule, @SourceType, @SourceDocumentId,
 @FiscalYearId, @FiscalPeriodId, @TotalDebit, @TotalCredit,
 NULL, NULL, 0, SYSUTCDATETIME(), @UserId);";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var tx = await cn.BeginTransactionAsync(ct);

            try
            {
                using var cmd = new SqlCommand(sql, cn, (SqlTransaction)tx);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                cmd.Parameters.AddWithValue("@JournalHeaderId", h.JournalHeaderId);
                cmd.Parameters.AddWithValue("@JournalId", h.JournalId);
                cmd.Parameters.AddWithValue("@JeNo", h.JeNo);
                cmd.Parameters.AddWithValue("@JeDate", h.JeDate);
                cmd.Parameters.AddWithValue("@Description", (object?)h.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", h.Status);
                cmd.Parameters.AddWithValue("@SourceModule", DBNull.Value);
                cmd.Parameters.AddWithValue("@SourceType", DBNull.Value);
                cmd.Parameters.AddWithValue("@SourceDocumentId", DBNull.Value);
                cmd.Parameters.AddWithValue("@FiscalYearId", (object?)h.FiscalYearId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FiscalPeriodId", (object?)h.FiscalPeriodId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TotalDebit", h.TotalDebit);
                cmd.Parameters.AddWithValue("@TotalCredit", h.TotalCredit);
                cmd.Parameters.AddWithValue("@UserId", userId);

                await cmd.ExecuteNonQueryAsync(ct);
                await tx.CommitAsync(ct);
                return h.JournalHeaderId;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task InsertLinesAsync(Guid companyId, Guid userId, Guid headerId, IReadOnlyList<JournalDraftLine> lines, CancellationToken ct)
        {
            const string sql = @"
INSERT INTO acc.JournalLine
(CompanyId, JournalLineId, JournalHeaderId, LineNo, AccountId, Debit, Credit,
 CurrencyId, FxRate, AmountFC, CostCenterId, Notes,
 IsDeleted, CreatedAt, CreatedBy)
VALUES
(@CompanyId, @JournalLineId, @JournalHeaderId, @LineNo, @AccountId, @Debit, @Credit,
 @CurrencyId, @FxRate, @AmountFC, @CostCenterId, @Notes,
 0, SYSUTCDATETIME(), @UserId);";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var tx = await cn.BeginTransactionAsync(ct);

            try
            {
                foreach (var l in lines)
                {
                    using var cmd = new SqlCommand(sql, cn, (SqlTransaction)tx);

                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@JournalLineId", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@JournalHeaderId", headerId);
                    cmd.Parameters.AddWithValue("@LineNo", l.LineNo);
                    cmd.Parameters.AddWithValue("@AccountId", l.AccountId);
                    cmd.Parameters.AddWithValue("@Debit", l.Debit);
                    cmd.Parameters.AddWithValue("@Credit", l.Credit);

                    cmd.Parameters.AddWithValue("@CurrencyId", (object?)l.CurrencyId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FxRate", (object?)l.FxRate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AmountFC", (object?)l.AmountFC ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostCenterId", (object?)l.CostCenterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notes", (object?)l.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task UpdatePostedAsync(Guid companyId, Guid userId, Guid headerId, CancellationToken ct)
        {
            const string sql = @"
UPDATE acc.JournalHeader
SET Status = N'Posted',
    PostedAt = SYSUTCDATETIME(),
    PostedBy = @UserId,
    UpdatedAt = SYSUTCDATETIME(),
    UpdatedBy = @UserId
WHERE CompanyId=@CompanyId AND JournalHeaderId=@HeaderId AND IsDeleted=0;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            cmd.Parameters.AddWithValue("@UserId", userId);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<IReadOnlyList<JournalHeaderDto>> ListHeadersAsync(Guid companyId, string? status, DateTime? from, DateTime? to, CancellationToken ct)
        {
            const string sql = @"
SELECT TOP(500)
  CompanyId, JournalHeaderId, JournalId, JeNo, JeDate, Description, Status,
  FiscalYearId, FiscalPeriodId, TotalDebit, TotalCredit, PostedAt, PostedBy
FROM acc.JournalHeader
WHERE CompanyId=@CompanyId AND IsDeleted=0
  AND (@Status IS NULL OR Status=@Status)
  AND (@From IS NULL OR JeDate>=@From)
  AND (@To IS NULL OR JeDate<=@To)
ORDER BY JeDate DESC, JeNo DESC;";

            var list = new List<JournalHeaderDto>();

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@From", (object?)from ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@To", (object?)to ?? DBNull.Value);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                list.Add(new JournalHeaderDto(
                    r.GetGuid(0),
                    r.GetGuid(1),
                    r.GetGuid(2),
                    r.GetString(3),
                    r.GetDateTime(4),
                    r.IsDBNull(5) ? null : r.GetString(5),
                    r.GetString(6),
                    r.IsDBNull(7) ? null : r.GetGuid(7),
                    r.IsDBNull(8) ? null : r.GetGuid(8),
                    r.GetDecimal(9),
                    r.GetDecimal(10),
                    r.IsDBNull(11) ? null : r.GetDateTime(11),
                    r.IsDBNull(12) ? null : r.GetGuid(12)
                ));
            }

            return list;
        }

        public async Task<IReadOnlyList<JournalLineDto>> GetLinesAsync(Guid companyId, Guid headerId, CancellationToken ct)
        {
            const string sql = @"
SELECT CompanyId, JournalLineId, JournalHeaderId, LineNo, AccountId, Debit, Credit,
       CurrencyId, FxRate, AmountFC, CostCenterId, Notes
FROM acc.JournalLine
WHERE CompanyId=@CompanyId AND JournalHeaderId=@HeaderId AND IsDeleted=0
ORDER BY LineNo;";

            var list = new List<JournalLineDto>();

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);

            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
            {
                list.Add(new JournalLineDto(
                    r.GetGuid(0),
                    r.GetGuid(1),
                    r.GetGuid(2),
                    r.GetInt32(3),
                    r.GetGuid(4),
                    r.GetDecimal(5),
                    r.GetDecimal(6),
                    r.IsDBNull(7) ? null : r.GetGuid(7),
                    r.IsDBNull(8) ? null : r.GetDecimal(8),
                    r.IsDBNull(9) ? null : r.GetDecimal(9),
                    r.IsDBNull(10) ? null : r.GetGuid(10),
                    r.IsDBNull(11) ? null : r.GetString(11)
                ));
            }

            return list;
        }
    }
}
