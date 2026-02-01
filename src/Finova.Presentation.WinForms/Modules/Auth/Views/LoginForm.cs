using System;
using System.Drawing;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Auth.Contracts;

namespace Finova.Presentation.WinForms.Modules.Auth.Views
{
    public sealed class LoginForm : Form, ILoginView
    {
        private readonly TextBox _txtUser;
        private readonly TextBox _txtPass;
        private readonly Button _btnSignIn;
        private readonly Button _btnExit;
        private readonly Label _lblTitle;
        private readonly Label _lblMsg;

        public event EventHandler? SignInRequested;
        public event EventHandler? ExitRequested;

        public string? UserName => _txtUser.Text;
        public string Password => _txtPass.Text;

        public LoginForm()
        {
            Text = "Finova ERP - Login";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 300);

            _lblTitle = new Label
            {
                Text = "Sign in",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 18)
            };
            Controls.Add(_lblTitle);

            var lblUser = new Label { Text = "Username", AutoSize = true, Location = new Point(24, 70) };
            Controls.Add(lblUser);

            _txtUser = new TextBox { Width = 360, Location = new Point(24, 92) };
            Controls.Add(_txtUser);

            var lblPass = new Label { Text = "Password", AutoSize = true, Location = new Point(24, 132) };
            Controls.Add(lblPass);

            _txtPass = new TextBox { Width = 360, Location = new Point(24, 154), UseSystemPasswordChar = true };
            Controls.Add(_txtPass);

            _lblMsg = new Label
            {
                Text = "",
                ForeColor = Color.DarkRed,
                AutoSize = false,
                Width = 360,
                Height = 38,
                Location = new Point(24, 190)
            };
            Controls.Add(_lblMsg);

            _btnSignIn = new Button
            {
                Text = "Sign in",
                Width = 120,
                Height = 34,
                Location = new Point(24, 238)
            };
            _btnSignIn.Click += (_, __) => SignInRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnSignIn);

            _btnExit = new Button
            {
                Text = "Exit",
                Width = 120,
                Height = 34,
                Location = new Point(264, 238)
            };
            _btnExit.Click += (_, __) => ExitRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnExit);

            // Enter triggers sign-in
            _txtPass.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SignInRequested?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            // Default dev credentials (optional convenience)
            _txtUser.Text = "admin";
            _txtPass.Text = "Admin@123";
        }

        public void SetBusy(bool isBusy)
        {
            _btnSignIn.Enabled = !isBusy;
            _btnExit.Enabled = !isBusy;
            _txtUser.ReadOnly = isBusy;
            _txtPass.ReadOnly = isBusy;

            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
        }

        public void ShowError(string message)
        {
            _lblMsg.Text = message ?? "Error";
        }

        public void CloseView()
        {
            Close();
        }
    }
}
