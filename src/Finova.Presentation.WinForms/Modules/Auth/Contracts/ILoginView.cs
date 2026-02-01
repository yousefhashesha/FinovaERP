using System;

namespace Finova.Presentation.WinForms.Modules.Auth.Contracts
{
    public interface ILoginView
    {
        event EventHandler SignInRequested;
        event EventHandler ExitRequested;

        string? UserName { get; }
        string Password { get; }

        void SetBusy(bool isBusy);
        void ShowError(string message);
        void CloseView();
    }
}
