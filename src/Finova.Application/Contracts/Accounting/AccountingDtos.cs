using System;
using System.Collections.Generic;

namespace Finova.Application.Contracts.Accounting
{
    public sealed record AccountDto(
        Guid CompanyId,
        Guid AccountId,
        string AccountCode,
        string AccountName,
        string AccountType,
        Guid? ParentAccountId,
        bool IsPosting,
        string NormalBalance, // 'DR' or 'CR' (nchar(2))
        byte Level,
        bool IsActive
    );

    public sealed record JournalDto(
        Guid CompanyId,
        Guid JournalId,
        string JournalCode,
        string JournalName,
        bool IsActive
    );

    public sealed record JournalHeaderDto(
        Guid CompanyId,
        Guid JournalHeaderId,
        Guid JournalId,
        string JeNo,
        DateTime JeDate,
        string? Description,
        string Status,
        Guid? FiscalYearId,
        Guid? FiscalPeriodId,
        decimal TotalDebit,
        decimal TotalCredit,
        DateTime? PostedAt,
        Guid? PostedBy
    );

    public sealed record JournalLineDto(
        Guid CompanyId,
        Guid JournalLineId,
        Guid JournalHeaderId,
        int LineNo,
        Guid AccountId,
        decimal Debit,
        decimal Credit,
        Guid? CurrencyId,
        decimal? FxRate,
        decimal? AmountFC,
        Guid? CostCenterId,
        string? Notes
    );

    public sealed record JournalDraftLine(
        int LineNo,
        Guid AccountId,
        decimal Debit,
        decimal Credit,
        string? Notes = null,
        Guid? CurrencyId = null,
        decimal? FxRate = null,
        decimal? AmountFC = null,
        Guid? CostCenterId = null
    );

    public sealed record JournalDraft(
        Guid JournalId,
        DateTime JeDate,
        string? Description,
        string? SourceModule,
        string? SourceType,
        Guid? SourceDocumentId,
        List<JournalDraftLine> Lines
    );

    public sealed record PostResult(bool Success, string Message, Guid? JournalHeaderId = null);
}
