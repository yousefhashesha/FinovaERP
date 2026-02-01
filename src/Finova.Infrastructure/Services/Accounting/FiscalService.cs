using System;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Accounting;
using Finova.Infrastructure.Repositories.Accounting;

namespace Finova.Infrastructure.Services.Accounting
{
    public sealed class FiscalService : IFiscalService
    {
        private readonly FiscalRepository _repo;
        public FiscalService(FiscalRepository repo) => _repo = repo;

        public Task<(Guid? FiscalPeriodId, Guid? FiscalYearId, bool IsClosed)> ResolvePeriodAsync(Guid companyId, DateTime date, CancellationToken ct = default)
            => _repo.ResolvePeriodAsync(companyId, date, ct);
    }
}
