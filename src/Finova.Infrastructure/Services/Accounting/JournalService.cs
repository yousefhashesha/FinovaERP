using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Accounting;
using Finova.Infrastructure.Repositories.Accounting;

namespace Finova.Infrastructure.Services.Accounting
{
    public sealed class JournalService : IJournalService
    {
        private readonly JournalRepository _journalRepo;
        private readonly AccountRepository _accountRepo;
        private readonly IFiscalService _fiscal;

        public JournalService(JournalRepository journalRepo, AccountRepository accountRepo, IFiscalService fiscal)
        {
            _journalRepo = journalRepo;
            _accountRepo = accountRepo;
            _fiscal = fiscal;
        }

        public Task<IReadOnlyList<JournalDto>> GetJournalsAsync(Guid companyId, CancellationToken ct = default)
            => _journalRepo.GetJournalsAsync(companyId, ct);

        public async Task<Guid> SaveDraftAsync(Guid companyId, Guid userId, JournalDraft draft, CancellationToken ct = default)
        {
            if (draft.Lines == null || draft.Lines.Count == 0)
                throw new InvalidOperationException("Journal must contain at least one line.");

            // Validate lines + totals
            decimal td = 0m, tc = 0m;
            foreach (var l in draft.Lines)
            {
                if (l.Debit < 0 || l.Credit < 0) throw new InvalidOperationException("Negative amounts are not allowed.");
                if (l.Debit > 0 && l.Credit > 0) throw new InvalidOperationException("A line cannot have both Debit and Credit.");
                if (l.Debit == 0 && l.Credit == 0) throw new InvalidOperationException("A line must have Debit or Credit.");
                td += l.Debit;
                tc += l.Credit;

                // Account must be posting & active
                var acc = await _accountRepo.GetByIdAsync(companyId, l.AccountId, ct);
                if (acc is null) throw new InvalidOperationException("Account not found.");
                if (!acc.IsActive) throw new InvalidOperationException($"Account inactive: {acc.AccountCode} - {acc.AccountName}");
                if (!acc.IsPosting) throw new InvalidOperationException($"Cannot post to a non-posting account: {acc.AccountCode} - {acc.AccountName}");
            }

            // Double entry rule
            if (Math.Round(td, 4) != Math.Round(tc, 4))
                throw new InvalidOperationException($"Unbalanced entry. TotalDebit={td:0.####}, TotalCredit={tc:0.####}");

            // Period resolve
            var (periodId, yearId, isClosed) = await _fiscal.ResolvePeriodAsync(companyId, draft.JeDate, ct);
            if (periodId is null) throw new InvalidOperationException("No fiscal period found for JeDate.");
            if (isClosed) throw new InvalidOperationException("Fiscal period is closed.");

            // Build header
            var headerId = Guid.NewGuid();
            var jeNo = "JE-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            var header = new JournalHeaderDto(
                companyId,
                headerId,
                draft.JournalId,
                jeNo,
                draft.JeDate,
                draft.Description,
                Status: "Draft",
                FiscalYearId: yearId,
                FiscalPeriodId: periodId,
                TotalDebit: td,
                TotalCredit: tc,
                PostedAt: null,
                PostedBy: null
            );

            await _journalRepo.InsertHeaderAsync(companyId, userId, header, ct);
            await _journalRepo.InsertLinesAsync(companyId, userId, headerId, draft.Lines, ct);

            return headerId;
        }

        public async Task<PostResult> PostAsync(Guid companyId, Guid userId, Guid journalHeaderId, CancellationToken ct = default)
        {
            // Posting engine in this phase:
            // - no edits here (assume draft is correct)
            // - simply mark header as Posted
            await _journalRepo.UpdatePostedAsync(companyId, userId, journalHeaderId, ct);
            return new PostResult(true, "Posted", journalHeaderId);
        }

        public Task<IReadOnlyList<JournalHeaderDto>> ListHeadersAsync(Guid companyId, string? status, DateTime? from, DateTime? to, CancellationToken ct = default)
            => _journalRepo.ListHeadersAsync(companyId, status, from, to, ct);

        public Task<IReadOnlyList<JournalLineDto>> GetLinesAsync(Guid companyId, Guid journalHeaderId, CancellationToken ct = default)
            => _journalRepo.GetLinesAsync(companyId, journalHeaderId, ct);
    }
}
