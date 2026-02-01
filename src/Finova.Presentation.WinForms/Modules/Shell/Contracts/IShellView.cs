using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Finova.Presentation.WinForms.Modules.Shell.Contracts
{
    public interface IShellView
    {
        event EventHandler<string>? NavigationRequested;
        event EventHandler? LogoutRequested;

        event EventHandler<string>? SearchRequested;
        event EventHandler<Guid>? CompanyChanged;

        void SetHeader(string companyName, string userDisplayName, string periodText);
        void SetCompanies(IEnumerable<(Guid Id, string Name)> companies, Guid? selectedCompanyId);

        void ApplyNavigation(IEnumerable<ShellNavItem> items);

        // Stable tab host API
        void OpenTab(string key, string title, Control content, bool activate = true);
    }
}
