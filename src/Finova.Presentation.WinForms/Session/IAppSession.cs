using System;
using System.Collections.Generic;

namespace Finova.Presentation.WinForms.Session
{
    /// <summary>
    /// Minimal session contract to satisfy compilation and support header/company switch.
    /// Extend later when wiring real RBAC + periods.
    /// </summary>
    public interface IAppSession
    {
        Guid? CurrentCompanyId { get; }
        Guid? CurrentUserId { get; }

        string UserName { get; }
        string DisplayName { get; }

        IReadOnlyList<CompanyDto> Companies { get; }

        void SetCompany(Guid companyId);
    }
}
