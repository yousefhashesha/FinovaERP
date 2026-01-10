using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace FinovaERP.Presentation.Forms
{
    /// <summary>
    /// Main application form with modern navigation
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        
        // Forms - initialized when needed
        private LoginForm? _loginForm;
        private DashboardForm? _dashboardForm;
        private ItemListForm? _itemListForm;

        public MainForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            InitializeComponent();
            ConfigureMainForm();
            ShowLoginForm();
        }

        private void ConfigureMainForm()
        {
            this.Text = "FinovaERP - Enterprise Resource Planning System";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 10F);
            this.IsMdiContainer = false; // Fixed: Remove MDI conflict
        }

        private void ShowLoginForm()
        {
            try
            {
                _loginForm = _serviceProvider.GetRequiredService<LoginForm>();
                var result = _loginForm.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    // Login successful, show Dashboard
                    ShowDashboard();
                }
                else
                {
                    // Login cancelled, close application
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing login form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ShowDashboard()
        {
            try
            {
                if (_dashboardForm == null || _dashboardForm.IsDisposed)
                {
                    _dashboardForm = _serviceProvider.GetRequiredService<DashboardForm>();
                }

                // Show dashboard as main form (not MDI child)
                _dashboardForm.Show();
                this.Hide(); // Hide main form
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowItemList()
        {
            try
            {
                if (_itemListForm == null || _itemListForm.IsDisposed)
                {
                    _itemListForm = new ItemListForm(_serviceProvider);
                }

                _itemListForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing item list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Ensure all child forms are closed properly
            if (_dashboardForm != null && !_dashboardForm.IsDisposed)
            {
                _dashboardForm.Close();
            }

            if (_itemListForm != null && !_itemListForm.IsDisposed)
            {
                _itemListForm.Close();
            }

            base.OnFormClosing(e);
        }
    }
}
