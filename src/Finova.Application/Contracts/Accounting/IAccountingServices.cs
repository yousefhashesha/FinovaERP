using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finova.Application.Contracts.Accounting
{
    public interface IAccountService
    {
        Task<IReadOnlyList<AccountDto>> GetChartAsync(Guid companyId, bool includeInactive, CancellationToken ct = default);
        Task<AccountDto?> GetByIdAsync(Guid companyId, Guid accountId, CancellationToken ct = default);

        Task<Guid> CreateAsync(Guid companyId, string code, string name, string type, Guid? parentId, bool isPosting, string normalBalance, byte level, Guid? userId, CancellationToken ct = default);
        Task UpdateAsync(Guid companyId, Guid accountId, string code, string name, string type, Guid? parentId, bool isPosting, string normalBalance, byte level, bool isActive, Guid? userId, CancellationToken ct = default);
    }

    public interface IJournalService
    {
        Task<IReadOnlyList<JournalDto>> GetJournalsAsync(Guid companyId, CancellationToken ct = default);

        Task<Guid> SaveDraftAsync(Guid companyId, Guid userId, JournalDraft draft, CancellationToken ct = default);
        Task<PostResult> PostAsync(Guid companyId, Guid userId, Guid journalHeaderId, CancellationToken ct = default);

        Task<IReadOnlyList<JournalHeaderDto>> ListHeadersAsync(Guid companyId, string? status, DateTime? from, DateTime? to, CancellationToken ct = default);
        Task<IReadOnlyList<JournalLineDto>> GetLinesAsync(Guid companyId, Guid journalHeaderId, CancellationToken ct = default);
    }

    public interface IFiscalService
    {
        Task<(Guid? FiscalPeriodId, Guid? FiscalYearId, bool IsClosed)> ResolvePeriodAsync(Guid companyId, DateTime date, CancellationToken ct = default);
    }
}
