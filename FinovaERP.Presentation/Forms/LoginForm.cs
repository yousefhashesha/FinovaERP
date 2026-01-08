using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Application.Services;
using FinovaERP.Infrastructure.Data;
using FinovaERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinovaERP.Presentation.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IServiceProvider _serviceProvider;

        public LoginForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            
            // Get services from DI container
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await PerformLoginAsync();
        }

        private async Task PerformLoginAsync()
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Validate input
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(""Please enter username"", ""Login Error"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(""Please enter password"", ""Login Error"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                // Show loading indicator
                btnLogin.Enabled = false;
                btnLogin.Text = ""Logging in..."";
                this.Cursor = Cursors.WaitCursor;

                // Attempt login
                var result = await _authenticationService.LoginAsync(username, password);

                if (result.Success)
                {
                    // Login successful
                    this.Hide();
                    
                    // Show main form
                    var mainForm = _serviceProvider.GetRequiredService<MainForm>();
                    mainForm.CurrentUser = result.User;
                    mainForm.CurrentCompanyId = result.CompanyId ?? 0;
                    mainForm.SessionToken = result.Token;
                    
                    mainForm.Show();
                }
                else
                {
                    // Login failed
                    MessageBox.Show(result.ErrorMessage, ""Login Failed"", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // Clear password field
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($""An error occurred during login: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reset UI
                btnLogin.Enabled = true;
                btnLogin.Text = ""Login"";
                this.Cursor = Cursors.Default;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnLogin.PerformClick();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set default credentials for testing (remove in production)
            txtUsername.Text = ""admin"";
            txtPassword.Text = ""Admin123!"";
            
            // Focus on login button
            btnLogin.Focus();
        }

        // Test connection button (for debugging)
        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                var dbContext = _serviceProvider.GetRequiredService<FinovaDbContext>();
                var canConnect = await dbContext.Database.CanConnectAsync();
                
                MessageBox.Show(canConnect ? ""Database connection successful!"" : ""Cannot connect to database!"", 
                    ""Connection Test"", 
                    MessageBoxButtons.OK, 
                    canConnect ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($""Connection error: {ex.Message}"", ""Connection Test"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
