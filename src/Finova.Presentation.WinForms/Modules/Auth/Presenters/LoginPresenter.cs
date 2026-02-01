using System;
using System.Threading.Tasks;
using Finova.Application.Contracts.Auth;
using Finova.Application.Contracts.Context;
using Finova.Presentation.WinForms.Modules.Auth.Contracts;

namespace Finova.Presentation.WinForms.Modules.Auth.Presenters
{
    public sealed class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly IAuthService _auth;
        private readonly ICompanyService _companies;
        private readonly IPermissionService _perms;
        private readonly IAppSession _session;

        public event EventHandler? SignedIn;

        public LoginPresenter(ILoginView view,
            IAuthService auth,
            ICompanyService companies,
            IPermissionService perms,
            IAppSession session)
        {
            _view = view;
            _auth = auth;
            _companies = companies;
            _perms = perms;
            _session = session;

            _view.SignInRequested += async (_, __) => await SignInAsync();
            _view.ExitRequested += (_, __) => _view.CloseView();
        }

        private async Task SignInAsync()
        {
            _view.SetBusy(true);

            try
            {
                var username = _view.UserName?.Trim() ?? "";
                var password = _view.Password ?? "";

                var result = await _auth.SignInAsync(username, password);
                if (!result.Success || result.UserId is null)
                {
                    _view.ShowError(result.Message);
                    return;
                }

                var userId = result.UserId.Value;

                var defaultCompany = await _companies.GetDefaultCompanyAsync(userId);
                if (defaultCompany is null)
                {
                    _view.ShowError("No company is assigned to this user (sec.UserCompany).");
                    return;
                }

                var permissions = await _perms.GetUserPermissionCodesAsync(userId);

                _session.Start(
                    userId,
                    username,
                    result.DisplayName ?? username,
                    defaultCompany.CompanyId,
                    defaultCompany.CompanyName,
                    permissions
                );

                SignedIn?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowError("Login failed: " + ex.Message);
            }
            finally
            {
                _view.SetBusy(false);
            }
        }
    }
}
