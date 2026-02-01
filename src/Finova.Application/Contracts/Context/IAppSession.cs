using System;
using System.Collections.Generic;

namespace Finova.Application.Contracts.Context
{
    public interface IAppSession
    {
        Guid UserId { get; }
        string UserName { get; }

        // Added: required by Shell
        string UserDisplayName { get; }
        string PeriodText { get; }

        // Companies for company switcher
        IReadOnlyList<CompanyDto> Companies { get; }

        Guid? CurrentCompanyId { get; }

        void SetCompany(Guid companyId);
        void SetUser(Guid userId, string userName, string displayName);
        void SetPeriod(string periodText);
        void SetCompanies(IReadOnlyList<CompanyDto> companies, Guid? currentCompanyId);
        void Clear();
    }

    public sealed record CompanyDto(
        Guid CompanyId,
        string CompanyCode,
        string CompanyName
    );
}
