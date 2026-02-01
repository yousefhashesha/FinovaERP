using System;
using System.Collections.Generic;
using System.Linq;

namespace Finova.Presentation.WinForms.Session
{
    public sealed class AppSession : IAppSession
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; } = string.Empty;

        public string UserDisplayName { get; private set; } = string.Empty;
        public string PeriodText { get; private set; } = "Period";

        public IReadOnlyList<CompanyDto> Companies { get; private set; } = Array.Empty<CompanyDto>();

        public Guid? CurrentCompanyId { get; private set; }

        public void SetCompany(Guid companyId)
        {
            CurrentCompanyId = companyId;
        }

        public void SetUser(Guid userId, string userName, string displayName)
        {
            UserId = userId;
            UserName = userName ?? string.Empty;
            UserDisplayName = string.IsNullOrWhiteSpace(displayName) ? UserName : displayName;
        }

        public void SetPeriod(string periodText)
        {
            PeriodText = string.IsNullOrWhiteSpace(periodText) ? "Period" : periodText;
        }

        public void SetCompanies(IReadOnlyList<CompanyDto> companies, Guid? currentCompanyId)
        {
            Companies = companies ?? Array.Empty<CompanyDto>();
            CurrentCompanyId = currentCompanyId ?? Companies.FirstOrDefault()?.CompanyId;
        }

        public void Clear()
        {
            UserId = Guid.Empty;
            UserName = string.Empty;
            UserDisplayName = string.Empty;
            PeriodText = "Period";
            Companies = Array.Empty<CompanyDto>();
            CurrentCompanyId = null;
        }
    }
}
