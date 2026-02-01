using System;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Auth.Presenters;
using Finova.Presentation.WinForms.Modules.Auth.Views;
using Finova.Presentation.WinForms.Modules.Shell.Presenters;
using Finova.Presentation.WinForms.Modules.Shell.Views;
using Finova.Presentation.WinForms.Theme;
using Microsoft.Extensions.DependencyInjection;

namespace Finova.Presentation.WinForms.Bootstrap
{
    public sealed class FinovaAppContext : ApplicationContext
    {
        private readonly IServiceProvider _sp;

        private LoginForm? _loginForm;
        private ShellForm? _shellForm;

        private bool _isLoggingOut;

        public FinovaAppContext(IServiceProvider sp)
        {
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));
            ShowLogin();
        }

        private void ShowLogin()
        {
            _isLoggingOut = false;

            _loginForm = _sp.GetRequiredService<LoginForm>();

            // Create presenter using SAME form instance
            var loginPresenter = ActivatorUtilities.CreateInstance<LoginPresenter>(_sp, _loginForm);

            loginPresenter.SignedIn += (_, __) =>
            {
                if (_loginForm is null) return;

                _loginForm.BeginInvoke(new Action(() =>
                {
                    _loginForm.Hide();
                    ShowShell();
                }));
            };

            _loginForm.FormClosed += (_, __) =>
            {
                // Close app if login is closed without sign-in
                ExitThread();
            };

            _loginForm.Show();
        }

        private void ShowShell()
        {
            _shellForm = _sp.GetRequiredService<ShellForm>();
            _shellForm.InjectTheme(_sp.GetRequiredService<ThemeService>());

            var shellPresenter = ActivatorUtilities.CreateInstance<ShellPresenter>(_sp, _shellForm);

            // Load header/companies/nav from DB via Session
            shellPresenter.InitializeFromSession();

            _shellForm.LogoutRequested += (_, __) =>
            {
                _isLoggingOut = true;
                _shellForm.BeginInvoke(new Action(() => _shellForm.Close()));
            };

            _shellForm.FormClosed += (_, __) =>
            {
                if (_isLoggingOut)
                {
                    _shellForm = null;
                    ShowLogin();
                    return;
                }

                ExitThread();
            };

            _shellForm.Show();
        }
    }
}
