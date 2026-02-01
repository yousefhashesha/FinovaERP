using System;
using System.Windows.Forms;
using Finova.Application.Contracts.Accounting;
using Finova.Application.Contracts.Auth;
using Finova.Application.Contracts.Context;
using Finova.Infrastructure.Data;
using Finova.Infrastructure.Repositories;
using Finova.Infrastructure.Repositories.Accounting;
using Finova.Infrastructure.Security;
using Finova.Infrastructure.Services;
using Finova.Infrastructure.Services.Accounting;
using Finova.Presentation.WinForms.Bootstrap;
using Finova.Presentation.WinForms.Config;
using Finova.Presentation.WinForms.Modules.Accounting.Presenters;
using Finova.Presentation.WinForms.Modules.Accounting.Views;
using Finova.Presentation.WinForms.Modules.Auth.Presenters;
using Finova.Presentation.WinForms.Modules.Auth.Views;
using Finova.Presentation.WinForms.Modules.Shell.Presenters;
using Finova.Presentation.WinForms.Modules.Shell.Views;
using Finova.Presentation.WinForms.Session;
using Finova.Presentation.WinForms.Theme;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finova.Presentation.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var modules = ConfigLoader.LoadModulesConfig();
            var permissions = ConfigLoader.LoadPermissionsConfig();

            var branding = config.GetSection("Branding").Get<BrandingOptions>() ?? new BrandingOptions();
            var theme = new ThemeService(branding);

            var services = new ServiceCollection();

            services.AddSingleton(config);
            services.AddSingleton(modules);
            services.AddSingleton(permissions);
            services.AddSingleton(theme);

            var cs = config.GetConnectionString("Finova")
                     ?? throw new InvalidOperationException("Missing ConnectionStrings:Finova in appsettings.json");

            services.AddSingleton(new SqlConnectionFactory(cs));

            // =========================
            // Security / Context
            // =========================
            services.AddSingleton<UserAuthRepository>();
            services.AddSingleton<CompanyRepository>();
            services.AddSingleton<PermissionRepository>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICompanyService, CompanyService>();
            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddSingleton<IAppSession, AppSession>();

            // =========================
            // Accounting (Phase 2)
            // =========================
            services.AddSingleton<AccountRepository>();
            services.AddSingleton<JournalRepository>();
            services.AddSingleton<FiscalRepository>();

            services.AddSingleton<IFiscalService, FiscalService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IJournalService, JournalService>();

            // =========================
            // Forms / Views
            // =========================
            services.AddTransient<LoginForm>();
            services.AddTransient<ShellForm>();

            // Accounting views (UserControls)
            services.AddTransient<AccountingHomeControl>();

            // =========================
            // Presenters
            // =========================
            services.AddTransient<LoginPresenter>();
            services.AddTransient<ShellPresenter>();

            services.AddTransient<AccountingHomePresenter>();

            using var sp = services.BuildServiceProvider();
            System.Windows.Forms.Application.Run(new FinovaAppContext(sp));
        }
    }
}
