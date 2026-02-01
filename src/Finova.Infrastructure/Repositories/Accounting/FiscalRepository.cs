using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Finova.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Repositories.Accounting
{
    public sealed class FiscalRepository
    {
        private readonly SqlConnectionFactory _factory;
        public FiscalRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<(Guid? PeriodId, Guid? YearId, bool IsClosed)> ResolvePeriodAsync(Guid companyId, DateTime date, CancellationToken ct)
        {
            // We only saw core.FiscalPeriod in discovery (FiscalYear may exist too).
            const string sql = @"
SELECT TOP(1)
  FiscalPeriodId, FiscalYearId, IsClosed
FROM core.FiscalPeriod
WHERE CompanyId=@CompanyId
  AND IsDeleted=0
  AND @D >= StartDate AND @D <= EndDate
ORDER BY StartDate DESC;";

            using var cn = (SqlConnection)_factory.Create();
            await cn.OpenAsync(ct);

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@D", date);

            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
            if (!await r.ReadAsync(ct))
                return (null, null, false);

            return (
                r.GetGuid(0),
                r.GetGuid(1),
                r.GetBoolean(2)
            );
        }
    }
}
