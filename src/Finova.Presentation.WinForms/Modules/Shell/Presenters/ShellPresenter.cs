using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Shell.Contracts;
using Finova.Presentation.WinForms.Session;

namespace Finova.Presentation.WinForms.Modules.Shell.Presenters
{
    public sealed class ShellPresenter
    {
        private readonly IShellView _view;
        private readonly IAppSession _session;

        public ShellPresenter(IShellView view, IAppSession session)
        {
            _view = view;
            _session = session;

            _view.NavigationRequested += OnNavigationRequested;
            _view.CompanyChanged += OnCompanyChanged;
            _view.LogoutRequested += (_, __) => { };
            _view.SearchRequested += (_, __) => { };
        }

        public void InitializeFromSession()
        {
            var companyName = GetCompanyName();
            _view.SetHeader(companyName, _session.UserDisplayName, _session.PeriodText);

            var companies = (_session.Companies ?? Array.Empty<CompanyDto>())
                .Select(c => (c.CompanyId, c.CompanyName))
                .ToList();

            _view.SetCompanies(companies, _session.CurrentCompanyId);

            // Navigation: enabled parameter required
            var nav = new List<ShellNavItem>
            {
                new ShellNavItem("home", "Home", true),
                new ShellNavItem("accounting", "Accounting", true),
                new ShellNavItem("logout", "Logout", true)
            };

            _view.ApplyNavigation(nav);

            // Default home tab
            _view.OpenTab("home", "Home", new Label { Text = "Home", AutoSize = true }, true);
        }

        private string GetCompanyName()
        {
            if (_session.CurrentCompanyId.HasValue)
            {
                var c = (_session.Companies ?? Array.Empty<CompanyDto>())
                    .FirstOrDefault(x => x.CompanyId == _session.CurrentCompanyId.Value);
                if (c != null) return c.CompanyName;
            }

            return (_session.Companies?.FirstOrDefault()?.CompanyName) ?? "Company";
        }

        private void OnCompanyChanged(object? sender, Guid companyId)
        {
            _session.SetCompany(companyId);
            _view.SetHeader(GetCompanyName(), _session.UserDisplayName, _session.PeriodText);
        }

        private void OnNavigationRequested(object? sender, string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            switch (key.Trim().ToLowerInvariant())
            {
                case "home":
                    _view.OpenTab("home", "Home", new Label { Text = "Home", AutoSize = true }, true);
                    break;

                case "accounting":
                    // For now placeholder until Phase 2.2 screens
                    _view.OpenTab("accounting", "Accounting", new Label { Text = "Accounting (next phase)", AutoSize = true }, true);
                    break;

                case "logout":
                    _view.OpenTab("logout", "Logout", new Label { Text = "Use Logout button.", AutoSize = true }, true);
                    break;

                default:
                    _view.OpenTab(key, key, new Label { Text = $"Module: {key}", AutoSize = true }, true);
                    break;
            }
        }
    }
}
